// Registro de dependencias de la capa de aplicación.
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Interfaces;
using USystem.Application.Services;

namespace USystem.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection Services)
    {
        // Los validadores de FluentValidation viven en esta capa
        Services.AddValidatorsFromAssembly(typeof(IAuditService).Assembly);
        Services.AddScoped<IAuditService, AuditService>();
        return Services;
    }
}
