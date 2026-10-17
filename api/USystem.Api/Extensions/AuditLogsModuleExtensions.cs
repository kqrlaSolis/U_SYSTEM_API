// Módulo AuditLogs: repositorio y servicio de consulta de la bitácora.
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;

namespace USystem.Api.Extensions;

public static class AuditLogsModuleExtensions
{
    public static IServiceCollection AddAuditLogsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        Services.AddScoped<IAuditLogService, AuditLogService>();
        return Services;
    }
}
