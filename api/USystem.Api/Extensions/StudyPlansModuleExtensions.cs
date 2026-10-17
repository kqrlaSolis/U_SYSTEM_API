// Módulo StudyPlans: repositorio y servicio de planes de estudio.
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;

namespace USystem.Api.Extensions;

public static class StudyPlansModuleExtensions
{
    public static IServiceCollection AddStudyPlansModule(this IServiceCollection Services)
    {
        Services.AddScoped<IStudyPlanRepository, StudyPlanRepository>();
        Services.AddScoped<IStudyPlanService, StudyPlanService>();
        return Services;
    }
}
