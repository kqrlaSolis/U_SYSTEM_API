// Módulos de catálogos de la Fase 1: carreras, materias y periodos.
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;

namespace USystem.Api.Extensions;

public static class CareCatalogModuleExtensions
{
    public static IServiceCollection AddCareersModule(this IServiceCollection Services)
    {
        Services.AddScoped<ICareerRepository, CareerRepository>();
        Services.AddScoped<ICareerService, CareerService>();
        return Services;
    }

    public static IServiceCollection AddSubjectsModule(this IServiceCollection Services)
    {
        Services.AddScoped<ISubjectRepository, SubjectRepository>();
        Services.AddScoped<ISubjectService, SubjectService>();
        return Services;
    }

    public static IServiceCollection AddAcademicTermsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IAcademicTermRepository, AcademicTermRepository>();
        Services.AddScoped<IAcademicTermService, AcademicTermService>();
        return Services;
    }
}
