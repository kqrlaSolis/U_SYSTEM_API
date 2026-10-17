// Módulo Users + autenticación: repositorios, servicios y validadores del módulo 01.
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;

namespace USystem.Api.Extensions;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection Services)
    {
        Services.AddScoped<IUserRepository, UserRepository>();
        Services.AddScoped<IRoleRepository, RoleRepository>();
        Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        Services.AddScoped<IUserService, UserService>();
        Services.AddScoped<IAuthService, AuthService>();

        return Services;
    }
}
