// Crea el usuario administrador inicial cuando la tabla Users está vacía.
// Se ejecuta con `dotnet run --project api/USystem.Api -- --seed-admin` (nunca en el arranque normal).
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using USystem.Application.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Persistence;

public class AdminUserSeeder
{
    public const string DefaultEmail = "admin@usystem.local";
    public const string DefaultUsername = "Administrator";
    public const string AdministratorRoleName = "Administrator";

    private readonly USystemDbContext _Context;
    private readonly IPasswordHasher _PasswordHasher;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AdminUserSeeder> _Logger;

    public AdminUserSeeder(
        USystemDbContext Context,
        IPasswordHasher PasswordHasher,
        IConfiguration Configuration,
        ILogger<AdminUserSeeder> Logger)
    {
        _Context = Context;
        _PasswordHasher = PasswordHasher;
        _Configuration = Configuration;
        _Logger = Logger;
    }

    // Devuelve el Id del administrador creado, o el existente si no había que crear nada
    public async Task<long?> SeedAsync(string Password, CancellationToken Cancellation = default)
    {
        var PolicyError = PasswordPolicy.Validate(Password);
        if (PolicyError is not null)
        {
            throw new InvalidOperationException(
                "La contraseña del administrador no cumple la política (8+, mayúscula, minúscula, número y símbolo).");
        }

        var Existing = await _Context.Users.FirstOrDefaultAsync(Cancellation);
        if (Existing is not null)
        {
            _Logger.LogInformation(
                "Ya existen usuarios (Id {Id}); no se crea el administrador inicial.",
                Existing.Id);
            return Existing.Id;
        }

        var Role = await _Context.Roles
            .FirstOrDefaultAsync(CurrentRole => CurrentRole.Name == AdministratorRoleName, Cancellation)
            ?? throw new InvalidOperationException(
                "Falta el rol Administrator. Ejecuta docs/DataModel/Scripts/99_SeedData.sql primero.");

        var Email = _Configuration["Seed:AdminEmail"] ?? DefaultEmail;
        var Username = _Configuration["Seed:AdminUsername"] ?? DefaultUsername;

        var NewUser = new User
        {
            FirstName = "System",
            PaternalLastName = "Administrator",
            Email = Email,
            Username = Username,
            PasswordHash = _PasswordHasher.Hash(Password),
            IsActive = true,
            MustChangePassword = true,
            CreatedAt = DateTime.UtcNow,
        };

        _Context.Users.Add(NewUser);
        await _Context.SaveChangesAsync(Cancellation);

        _Context.UserRoles.Add(new UserRole { UserId = NewUser.Id, RoleId = Role.Id });
        await _Context.SaveChangesAsync(Cancellation);

        _Logger.LogInformation(
            "Usuario administrador {Username} creado con Id {Id}; debe cambiar la contraseña en el primer ingreso.",
            Username,
            NewUser.Id);

        return NewUser.Id;
    }
}
