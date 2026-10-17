// Módulo Roles: repositorio y servicio de roles y permisos.
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;

namespace USystem.Api.Extensions;

public static class RolesModuleExtensions
{
    public static IServiceCollection AddRolesModule(this IServiceCollection Services)
    {
        Services.AddScoped<IRoleRepository, RoleRepository>();
        Services.AddScoped<IRoleService, RoleService>();
        return Services;
    }
}
