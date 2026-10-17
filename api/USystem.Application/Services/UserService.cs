// Reglas del módulo Users: unicidad, política de contraseñas, roles y anti-bloqueo del sistema.
using System.Globalization;
using System.Security.Cryptography;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class UserService : IUserService
{
    private const string AdministratorRoleName = "Administrator";

    private static readonly char[] _UpperCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] _LowerCharacters = "abcdefghijkmnopqrstuvwxyz".ToCharArray();
    private static readonly char[] _DigitCharacters = "23456789".ToCharArray();
    private static readonly char[] _SymbolCharacters = "!@#$%&*".ToCharArray();

    private readonly IUserRepository _UserRepository;
    private readonly IRoleRepository _RoleRepository;
    private readonly IRefreshTokenRepository _RefreshTokenRepository;
    private readonly IPasswordHasher _PasswordHasher;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;
    private readonly ISettingsProvider _Settings;

    public UserService(
        IUserRepository UserRepository,
        IRoleRepository RoleRepository,
        IRefreshTokenRepository RefreshTokenRepository,
        IPasswordHasher PasswordHasher,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService,
        ISettingsProvider Settings)
    {
        _UserRepository = UserRepository;
        _RoleRepository = RoleRepository;
        _RefreshTokenRepository = RefreshTokenRepository;
        _PasswordHasher = PasswordHasher;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
        _Settings = Settings;
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _UserRepository.GetPagedAsync(Query, Cancellation);
        var UserIds = Page.Items.Select(User => User.Id).ToArray();
        var RolesByUser = await _UserRepository.GetRoleNamesByUserIdsAsync(UserIds, Cancellation);

        var Items = Page.Items
            .Select(User => UserMapper.ToDto(
                User,
                RolesByUser.TryGetValue(User.Id, out var Roles) ? Roles : Array.Empty<string>()))
            .ToList();

        return PagedResult<UserDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<UserDto> GetByIdAsync(long Id, CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Users.NotFound");

        var Roles = await _UserRepository.GetRoleNamesAsync(User.Id, Cancellation);
        var Permissions = await _UserRepository.GetPermissionCodesAsync(User.Id, Cancellation);
        return UserMapper.ToDto(User, Roles, Permissions);
    }

    public async Task<CreatedUserResponse> CreateAsync(
        CreateUserRequest Request,
        CancellationToken Cancellation = default)
    {
        var Email = Request.Email.Trim();
        var Username = Request.Username.Trim();

        await EnsureUniqueAsync(Email, Username, null, Cancellation);

        var RoleIds = await ValidateRoleIdsAsync(Request.RoleIds, Cancellation);

        string TemporaryPassword;
        var PasswordWasGenerated = string.IsNullOrWhiteSpace(Request.TemporaryPassword);

        if (PasswordWasGenerated)
        {
            TemporaryPassword = GenerateTemporaryPassword();
        }
        else
        {
            TemporaryPassword = Request.TemporaryPassword!.Trim();
            var MinimumLength = await _Settings.GetIntAsync("Password.MinLength", 8, Cancellation);
            var PolicyError = PasswordPolicy.Validate(TemporaryPassword, MinimumLength);
            if (PolicyError is not null)
            {
                throw new ValidationException(PolicyError, nameof(Request.TemporaryPassword));
            }
        }

        var NewUser = new User
        {
            FirstName = Request.FirstName.Trim(),
            PaternalLastName = Request.PaternalLastName.Trim(),
            MaternalLastName = string.IsNullOrWhiteSpace(Request.MaternalLastName)
                ? null
                : Request.MaternalLastName.Trim(),
            Email = Email,
            Username = Username,
            Phone = string.IsNullOrWhiteSpace(Request.Phone) ? null : Request.Phone.Trim(),
            PasswordHash = _PasswordHasher.Hash(TemporaryPassword),
            MustChangePassword = true,
            IsActive = true,
        };

        await _UserRepository.AddAsync(NewUser, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _UserRepository.ReplaceRolesAsync(NewUser.Id, RoleIds, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Users",
            nameof(User),
            NewUser.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewUser.Username, NewUser.Email, RoleIds },
            Cancellation);

        var Roles = await _UserRepository.GetRoleNamesAsync(NewUser.Id, Cancellation);

        return new CreatedUserResponse
        {
            User = UserMapper.ToDto(NewUser, Roles),
            TemporaryPassword = PasswordWasGenerated ? TemporaryPassword : null,
        };
    }

    public async Task<UserDto> UpdateAsync(
        long Id,
        UpdateUserRequest Request,
        CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Users.NotFound");

        var Email = Request.Email.Trim();
        var Username = Request.Username.Trim();
        await EnsureUniqueAsync(Email, Username, User.Id, Cancellation);

        var Before = new
        {
            User.FirstName,
            User.PaternalLastName,
            User.MaternalLastName,
            User.Email,
            User.Username,
            User.Phone,
        };

        User.FirstName = Request.FirstName.Trim();
        User.PaternalLastName = Request.PaternalLastName.Trim();
        User.MaternalLastName = string.IsNullOrWhiteSpace(Request.MaternalLastName)
            ? null
            : Request.MaternalLastName.Trim();
        User.Email = Email;
        User.Username = Username;
        User.Phone = string.IsNullOrWhiteSpace(Request.Phone) ? null : Request.Phone.Trim();

        _UserRepository.Update(User);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Users",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { User.FirstName, User.PaternalLastName, User.MaternalLastName, User.Email, User.Username, User.Phone },
            Cancellation);

        var Roles = await _UserRepository.GetRoleNamesAsync(User.Id, Cancellation);
        return UserMapper.ToDto(User, Roles);
    }

    public async Task<UserDto> SetStatusAsync(
        long Id,
        bool IsActive,
        CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Users.NotFound");

        if (User.IsActive == IsActive)
        {
            throw new BusinessRuleException(IsActive ? "Users.AlreadyActive" : "Users.AlreadyInactive");
        }

        var Roles = await _UserRepository.GetRoleNamesAsync(User.Id, Cancellation);

        // No se puede dejar al sistema sin un Administrator activo
        if (!IsActive && Roles.Contains(AdministratorRoleName, StringComparer.Ordinal))
        {
            var ActiveAdministrators = await _UserRepository.CountActiveAdministratorsAsync(Cancellation);
            if (ActiveAdministrators <= 1)
            {
                throw new BusinessRuleException("Auth.LastAdministrator");
            }
        }

        User.IsActive = IsActive;
        _UserRepository.Update(User);

        if (!IsActive)
        {
            await _RefreshTokenRepository.RevokeAllForUserAsync(User.Id, DateTime.UtcNow, Cancellation);
        }

        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            IsActive ? "Activate" : "Deactivate",
            "Users",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            new { IsActive = !IsActive },
            new { IsActive },
            Cancellation);

        return UserMapper.ToDto(User, Roles);
    }

    public async Task<TemporaryPasswordResponse> ResetPasswordAsync(
        long Id,
        ResetUserPasswordRequest Request,
        CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Users.NotFound");

        string TemporaryPassword;
        if (string.IsNullOrWhiteSpace(Request.TemporaryPassword))
        {
            TemporaryPassword = GenerateTemporaryPassword();
        }
        else
        {
            TemporaryPassword = Request.TemporaryPassword!.Trim();
            var MinimumLength = await _Settings.GetIntAsync("Password.MinLength", 8, Cancellation);
            var PolicyError = PasswordPolicy.Validate(TemporaryPassword, MinimumLength);
            if (PolicyError is not null)
            {
                throw new ValidationException(PolicyError, nameof(Request.TemporaryPassword));
            }
        }

        User.PasswordHash = _PasswordHasher.Hash(TemporaryPassword);
        User.MustChangePassword = true;
        User.FailedAttempts = 0;
        User.LockedUntil = null;
        _UserRepository.Update(User);

        await _RefreshTokenRepository.RevokeAllForUserAsync(User.Id, DateTime.UtcNow, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "ResetPassword",
            "Users",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            null,
            null,
            Cancellation);

        return new TemporaryPasswordResponse { TemporaryPassword = TemporaryPassword };
    }

    public async Task<UserDto> AssignRolesAsync(
        long Id,
        IReadOnlyCollection<int> RoleIds,
        CancellationToken Cancellation = default)
    {
        var User = await _UserRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Users.NotFound");

        var ValidRoleIds = await ValidateRoleIdsAsync(RoleIds, Cancellation);
        var NewRoles = await _RoleRepository.GetByIdsAsync(ValidRoleIds, Cancellation);
        var CurrentRoles = await _UserRepository.GetRoleNamesAsync(User.Id, Cancellation);

        // Quitar el rol Administrator al último administrador activo deja el sistema sin acceso
        var LosesAdministrator = CurrentRoles.Contains(AdministratorRoleName, StringComparer.Ordinal)
            && !NewRoles.Any(Role => Role.Name == AdministratorRoleName);

        if (LosesAdministrator && User.IsActive)
        {
            var ActiveAdministrators = await _UserRepository.CountActiveAdministratorsAsync(Cancellation);
            if (ActiveAdministrators <= 1)
            {
                throw new BusinessRuleException("Auth.LastAdministrator");
            }
        }

        await _UserRepository.ReplaceRolesAsync(User.Id, ValidRoleIds, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "AssignRoles",
            "Users",
            nameof(User),
            User.Id.ToString(CultureInfo.InvariantCulture),
            new { Roles = CurrentRoles },
            new { Roles = NewRoles.Select(Role => Role.Name).ToArray() },
            Cancellation);

        var Roles = await _UserRepository.GetRoleNamesAsync(User.Id, Cancellation);
        var Permissions = await _UserRepository.GetPermissionCodesAsync(User.Id, Cancellation);
        return UserMapper.ToDto(User, Roles, Permissions);
    }

    private async Task EnsureUniqueAsync(
        string Email,
        string Username,
        long? ExcludeUserId,
        CancellationToken Cancellation)
    {
        if (await _UserRepository.ExistsByEmailAsync(Email, ExcludeUserId, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Users.DuplicateEmail",
                new Dictionary<string, string> { ["Email"] = Email });
        }

        if (await _UserRepository.ExistsByUsernameAsync(Username, ExcludeUserId, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Users.DuplicateUsername",
                new Dictionary<string, string> { ["Username"] = Username });
        }
    }

    private async Task<int[]> ValidateRoleIdsAsync(
        IReadOnlyCollection<int> RoleIds,
        CancellationToken Cancellation)
    {
        var DistinctRoleIds = RoleIds.Distinct().ToArray();
        if (DistinctRoleIds.Length == 0)
        {
            throw new ValidationException("Users.InvalidRole", "RoleIds");
        }

        var Roles = await _RoleRepository.GetByIdsAsync(DistinctRoleIds, Cancellation);
        if (Roles.Count != DistinctRoleIds.Length)
        {
            throw new ValidationException("Users.InvalidRole", "RoleIds");
        }

        return DistinctRoleIds;
    }

    // Cumple la política: mayúscula, minúscula, número y símbolo
    private static string GenerateTemporaryPassword()
    {
        var Characters = new char[12];
        Characters[0] = PickRandom(_UpperCharacters);
        Characters[1] = PickRandom(_LowerCharacters);
        Characters[2] = PickRandom(_DigitCharacters);
        Characters[3] = PickRandom(_SymbolCharacters);

        var AllCharacters = _UpperCharacters
            .Concat(_LowerCharacters)
            .Concat(_DigitCharacters)
            .Concat(_SymbolCharacters)
            .ToArray();

        for (var Index = 4; Index < Characters.Length; Index++)
        {
            Characters[Index] = PickRandom(AllCharacters);
        }

        return new string(Characters.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }

    private static char PickRandom(char[] Characters)
        => Characters[RandomNumberGenerator.GetInt32(Characters.Length)];
}
