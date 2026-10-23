// Módulos de operación académica de la Fase 2 (docentes y siguientes).
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;

namespace USystem.Api.Extensions;

public static class AcademicOperationsModuleExtensions
{
    public static IServiceCollection AddTeachersModule(this IServiceCollection Services)
    {
        Services.AddScoped<ITeacherRepository, TeacherRepository>();
        Services.AddScoped<ITeacherService, TeacherService>();
        return Services;
    }

    public static IServiceCollection AddStudentsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IStudentRepository, StudentRepository>();
        Services.AddScoped<IStudentService, StudentService>();
        return Services;
    }

    public static IServiceCollection AddCourseMaterialsModule(this IServiceCollection Services)
    {
        Services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
        Services.AddScoped<ICourseMaterialService, CourseMaterialService>();
        return Services;
    }

    public static IServiceCollection AddClassGroupsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IClassGroupRepository, ClassGroupRepository>();
        Services.AddScoped<IClassGroupService, ClassGroupService>();
        return Services;
    }

    public static IServiceCollection AddEnrollmentsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        Services.AddScoped<IEnrollmentService, EnrollmentService>();
        Services.AddScoped<IEnrollmentChecklistRepository, EnrollmentChecklistRepository>();
        Services.AddScoped<IEnrollmentChecklistService, EnrollmentChecklistService>();
        return Services;
    }

    public static IServiceCollection AddGradesModule(this IServiceCollection Services)
    {
        Services.AddScoped<IGradeRepository, GradeRepository>();
        Services.AddScoped<IGradeService, GradeService>();
        return Services;
    }

    public static IServiceCollection AddTranscriptsModule(this IServiceCollection Services)
    {
        Services.AddScoped<ITranscriptRepository, TranscriptRepository>();
        Services.AddScoped<ITranscriptService, TranscriptService>();
        return Services;
    }

    public static IServiceCollection AddAcademicProgressModule(this IServiceCollection Services)
    {
        Services.AddScoped<IAcademicProgressService, AcademicProgressService>();
        return Services;
    }

    public static IServiceCollection AddReEnrollmentsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IReEnrollmentService, ReEnrollmentService>();
        return Services;
    }

    public static IServiceCollection AddNotificationsModule(this IServiceCollection Services)
    {
        Services.AddScoped<INotificationRepository, NotificationRepository>();
        Services.AddScoped<INotificationService, NotificationService>();
        return Services;
    }

    public static IServiceCollection AddProceduresModule(this IServiceCollection Services)
    {
        Services.AddScoped<IProcedureRepository, ProcedureRepository>();
        Services.AddScoped<IProcedureService, ProcedureService>();
        return Services;
    }

    public static IServiceCollection AddPaymentsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IPaymentRepository, PaymentRepository>();
        Services.AddScoped<IPaymentService, PaymentService>();
        return Services;
    }

    public static IServiceCollection AddReportsModule(this IServiceCollection Services)
    {
        Services.AddScoped<IReportRepository, ReportRepository>();
        Services.AddScoped<IReportService, ReportService>();
        return Services;
    }
}
