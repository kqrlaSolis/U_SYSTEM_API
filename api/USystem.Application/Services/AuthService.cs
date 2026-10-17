// Reglas de autenticación: credenciales, bloqueo, refresh con rotación y recuperación.
using System.Globalization;
using Microsoft.Extensions.Logging;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _UserRepository;
    private readonly IRefreshTokenRepository _RefreshTokenRepository;
    private readonly IPasswordResetTokenRepository _PasswordResetTokenRepository;
    private readonly IPasswordHasher _PasswordHasher;
    private readonly ITokenGenerator _TokenGenerator;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;
    private readonly ISettingsProvider _Settings;
    private readonly ILogger<AuthService> _Logger;

    public AuthService(
        IUserRepository UserRepository,
        IRefreshTokenRepository RefreshTokenRepository,
        IPasswordResetTokenRepository PasswordResetTokenRepository,
        IPasswordHasher PasswordHasher,
        ITokenGenerator TokenGenerator,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService,
        ISettingsProvider Settings,
        ILogger<AuthService> Logger)
    {
        _UserRepository = UserRepository;
        _RefreshTokenRepository = RefreshTokenRepository;
        _PasswordResetTokenRepository = PasswordResetTokenRepository;
        _PasswordHasher = PasswordHasher;
        _TokenGenerator = TokenGenerator;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
        _Settings = Settings;
        _Logger = Logger;
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest Request,
        CancellationToken Cancellation = default)
    {
        var MaxAttempts = await _Settings.GetIntAsync("Login.MaxFailedAttempts", 5, Cancellation);
        var LockoutMinutes = await _Settings.GetIntAsync("Login.LockoutMinutes", 15, Cancellation);

        var User = await _UserRepository.GetByLoginAsync(Request.UsernameOrEmail.Trim(), Cancellation);

        // Mismo mensaje exista o no la cuenta: no se filtra qué usuarios hay
        if (User is null)
        {
            await _AuditService.LogAsync(
                "LoginFailed",
                "Auth",
                nameof(User),
                null,
                null,
                new { Request.UsernameOrEmail },
                Cancellation);
            throw new BusinessRuleException("Auth.InvalidCredentials");
        }

        EnsureUserCanAuthenticate(User);

        if (!_PasswordHasher.Verify(Request.Password, User.PasswordHash))
        {
            await RegisterFailedAttemptAsync(User, MaxAttempts, LockoutMinutes, Cancellation);
            throw new BusinessRuleException("Auth.InvalidCredentials");
        }

        var Now = DateTime.UtcNow;
        User.FailedAttempts = 0;
        User.LockedUntil = null;
        User.LastLoginAt = Now;

        if (_PasswordHasher.NeedsRehash(User.PasswordHash))
        {
            User.PasswordHash = _PasswordHasher.Hash(Request.Password);
        }

        _UserRepository.Update(User);

        var Response = await CreateSessionAsync(User, null, Cancellation);
        await _AuditService.LogAsync(
            "Login",
            "Auth",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { User.Username },
            Cancellation);

        return Response;
    }

    public async Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest Request,
        string? IpAddress = null,
        CancellationToken Cancellation = default)
    {
        var Hash = SecureToken.Hash(Request.RefreshToken);
        var Stored = await _RefreshTokenRepository.GetByHashAsync(Hash, Cancellation);
        var Now = DateTime.UtcNow;

        if (Stored is null || Stored.RevokedAt is not null || Stored.ExpiresAt <= Now)
        {
            throw new BusinessRuleException("Auth.RefreshTokenInvalid");
        }

        var User = await _UserRepository.GetByIdAsync(Stored.UserId, Cancellation)
            ?? throw new BusinessRuleException("Auth.RefreshTokenInvalid");

        EnsureUserCanAuthenticate(User);

        // Rotación: el refresh usado se revoca y se emite uno nuevo
        Stored.RevokedAt = Now;
        _RefreshTokenRepository.Update(Stored);

        return await CreateSessionAsync(User, IpAddress, Cancellation);
    }

    public async Task LogoutAsync(
        RefreshTokenRequest Request,
        long UserId,
        CancellationToken Cancellation = default)
    {
        var Hash = SecureToken.Hash(Request.RefreshToken);
        var Stored = await _RefreshTokenRepository.GetByHashAsync(Hash, Cancellation);

        // Idempotente: si el token ya no existe o no es del usuario, no se hace nada
        if (Stored is null || Stored.UserId != UserId || Stored.RevokedAt is not null)
        {
            return;
        }

        Stored.RevokedAt = DateTime.UtcNow;
        _RefreshTokenRepository.Update(Stored);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Logout",
            "Auth",
            nameof(User),
            UserId.ToString(CultureInfo.InvariantCulture),
            null,
            null,
            Cancellation);
    }

    public async Task ChangePasswordAsync(
        ChangePasswordRequest Request,
        long UserId,
        CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByIdAsync(UserId, Cancellation)
            ?? throw new NotFoundException("Users.NotFound");

        if (!_PasswordHasher.Verify(Request.CurrentPassword, User.PasswordHash))
        {
            throw new BusinessRuleException("Auth.CurrentPasswordInvalid");
        }

        var MinimumLength = await _Settings.GetIntAsync("Password.MinLength", 8, Cancellation);
        var PolicyError = PasswordPolicy.Validate(Request.NewPassword, MinimumLength);
        if (PolicyError is not null)
        {
            throw new ValidationException(PolicyError, nameof(Request.NewPassword));
        }

        User.PasswordHash = _PasswordHasher.Hash(Request.NewPassword);
        User.MustChangePassword = false;
        User.FailedAttempts = 0;
        User.LockedUntil = null;
        _UserRepository.Update(User);

        // Cambiar la contraseña cierra todas las sesiones abiertas
        await _RefreshTokenRepository.RevokeAllForUserAsync(User.Id, DateTime.UtcNow, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "ChangePassword",
            "Auth",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            null,
            null,
            Cancellation);
    }

    public async Task ForgotPasswordAsync(
        ForgotPasswordRequest Request,
        CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByEmailAsync(Request.Email.Trim(), Cancellation);

        // Respuesta genérica: no se revela si el correo existe
        if (User is null || !User.IsActive)
        {
            return;
        }

        var Minutes = await _Settings.GetIntAsync("PasswordReset.ExpirationMinutes", 30, Cancellation);
        var Token = SecureToken.Generate();
        var ExpiresAt = DateTime.UtcNow.AddMinutes(Minutes <= 0 ? 30 : Minutes);

        await _PasswordResetTokenRepository.AddAsync(
            new PasswordResetToken
            {
                UserId = User.Id,
                TokenHash = SecureToken.Hash(Token),
                ExpiresAt = ExpiresAt,
            },
            Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "ForgotPassword",
            "Auth",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            null,
            null,
            Cancellation);

        // Sin proveedor SMTP todavía: el token se registra para el ambiente de desarrollo
        _Logger.LogWarning(
            "Token de recuperación para {Email}: {Token} (vence {ExpiresAt:u})",
            User.Email,
            Token,
            ExpiresAt);
    }

    public async Task ResetPasswordAsync(
        ResetPasswordRequest Request,
        CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByEmailAsync(Request.Email.Trim(), Cancellation)
            ?? throw new BusinessRuleException("Auth.ResetTokenInvalid");

        var Hash = SecureToken.Hash(Request.Token);
        var Stored = await _PasswordResetTokenRepository.GetByHashAsync(Hash, Cancellation);
        var Now = DateTime.UtcNow;

        if (Stored is null
            || Stored.UserId != User.Id
            || Stored.UsedAt is not null
            || Stored.ExpiresAt <= Now)
        {
            throw new BusinessRuleException("Auth.ResetTokenInvalid");
        }

        var MinimumLength = await _Settings.GetIntAsync("Password.MinLength", 8, Cancellation);
        var PolicyError = PasswordPolicy.Validate(Request.NewPassword, MinimumLength);
        if (PolicyError is not null)
        {
            throw new ValidationException(PolicyError, nameof(Request.NewPassword));
        }

        User.PasswordHash = _PasswordHasher.Hash(Request.NewPassword);
        User.MustChangePassword = false;
        User.FailedAttempts = 0;
        User.LockedUntil = null;
        _UserRepository.Update(User);

        Stored.UsedAt = Now;
        _PasswordResetTokenRepository.Update(Stored);

        await _RefreshTokenRepository.RevokeAllForUserAsync(User.Id, Now, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "ResetPassword",
            "Auth",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            null,
            null,
            Cancellation);
    }

    private static void EnsureUserCanAuthenticate(User User)
    {
        if (!User.IsActive)
        {
            throw new BusinessRuleException("Auth.AccountInactive");
        }

        if (User.LockedUntil is null || User.LockedUntil <= DateTime.UtcNow)
        {
            return;
        }

        var RemainingMinutes = (int)Math.Ceiling((User.LockedUntil.Value - DateTime.UtcNow).TotalMinutes);
        throw new BusinessRuleException(
            "Auth.AccountLocked",
            new Dictionary<string, string>
            {
                ["Minutes"] = Math.Max(RemainingMinutes, 1).ToString(CultureInfo.InvariantCulture),
            });
    }

    private async Task RegisterFailedAttemptAsync(
        User User,
        int MaxAttempts,
        int LockoutMinutes,
        CancellationToken Cancellation)
    {
        User.FailedAttempts += 1;

        if (User.FailedAttempts >= MaxAttempts)
        {
            User.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes <= 0 ? 15 : LockoutMinutes);
            User.FailedAttempts = 0;
        }

        _UserRepository.Update(User);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "LoginFailed",
            "Auth",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { User.Username, User.FailedAttempts },
            Cancellation);
    }

    private async Task<LoginResponse> CreateSessionAsync(
        User User,
        string? IpAddress,
        CancellationToken Cancellation)
    {
        var Roles = await _UserRepository.GetRoleNamesAsync(User.Id, Cancellation);
        var Permissions = await _UserRepository.GetPermissionCodesAsync(User.Id, Cancellation);
        var AccessToken = _TokenGenerator.GenerateAccessToken(User, Roles, Permissions);

        var RefreshDays = await _Settings.GetIntAsync("Jwt.RefreshTokenDays", 7, Cancellation);
        var RefreshExpiresAt = DateTime.UtcNow.AddDays(RefreshDays <= 0 ? 7 : RefreshDays);
        var RefreshTokenValue = SecureToken.Generate();

        await _RefreshTokenRepository.AddAsync(
            new RefreshToken
            {
                UserId = User.Id,
                TokenHash = SecureToken.Hash(RefreshTokenValue),
                ExpiresAt = RefreshExpiresAt,
                IpAddress = IpAddress,
            },
            Cancellation);

        await _UnitOfWork.SaveChangesAsync(Cancellation);

        return new LoginResponse
        {
            AccessToken = AccessToken.Token,
            AccessTokenExpiresAt = AccessToken.ExpiresAt,
            RefreshToken = RefreshTokenValue,
            RefreshTokenExpiresAt = RefreshExpiresAt,
            MustChangePassword = User.MustChangePassword,
            User = UserMapper.ToDto(User, Roles, Permissions),
        };
    }
}
