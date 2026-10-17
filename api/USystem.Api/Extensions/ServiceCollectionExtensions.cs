// Registro de servicios de la API: controladores, Swagger, CORS, localización y capas internas.
using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using USystem.Api.Filters;
using USystem.Api.Localization;
using USystem.Api.Security;
using USystem.Application;
using USystem.Application.Common;
using USystem.Application.Interfaces;
using USystem.Infrastructure;

namespace USystem.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public const string CorsPolicyName = "USystemDesktop";

    public static IServiceCollection AddUSystemApi(
        this IServiceCollection Services,
        IConfiguration Configuration)
    {
        Services.AddHttpContextAccessor();

        Services.Configure<LocalizationOptions>(Configuration.GetSection(LocalizationOptions.SectionName));
        Services.AddSingleton<IErrorMessageProvider, JsonErrorMessageProvider>();
        Services.AddScoped<USystem.Domain.Interfaces.ICurrentUser, CurrentUserService>();

        Services.AddControllers(Options =>
            {
                // Valida el cuerpo con FluentValidation y conserva el código i18n del error
                Options.Filters.Add(new ValidationFilter());
            })
            .AddJsonOptions(Options =>
            {
                // Contrato de la API: propiedades JSON en PascalCase
                Options.JsonSerializerOptions.PropertyNamingPolicy = null;
                Options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        // Un JSON mal formado o un modelo inválido también responden con el sobre ApiResponse
        Services.Configure<ApiBehaviorOptions>(Options =>
        {
            Options.InvalidModelStateResponseFactory = Context =>
            {
                var Messages = Context.HttpContext.RequestServices
                    .GetRequiredService<IErrorMessageProvider>();
                var Language = Messages.ResolveLanguage(
                    Context.HttpContext.Request.Headers.AcceptLanguage.ToString());
                var Message = Messages.GetMessage("Api.InvalidBody", null, Language);

                var Errors = Context.ModelState
                    .Where(Entry => Entry.Value is not null && Entry.Value.Errors.Count > 0)
                    .SelectMany(Entry => Entry.Value!.Errors.Select(Error => new ApiError
                    {
                        Code = "Api.InvalidBody",
                        Field = Entry.Key,
                        Message = string.IsNullOrWhiteSpace(Error.ErrorMessage) ? Message : Error.ErrorMessage,
                    }))
                    .ToList();

                var Response = new ApiResponse<object>
                {
                    Success = false,
                    Message = Message,
                    Errors = Errors,
                    TraceId = Activity.Current?.Id ?? Context.HttpContext.TraceIdentifier,
                };

                return new BadRequestObjectResult(Response);
            };
        });

        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen(Options =>
        {
            Options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "U SYSTEM API",
                Version = "v1",
                Description = "Sistema administrativo de gestión escolar universitaria. JSON en PascalCase.",
            });

            // Swagger permite pegar el access token para probar los endpoints protegidos
            Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Pega solo el access token; el prefijo Bearer lo agrega Swagger.",
            });

            Options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });

        Services.AddCors(Options => Options.AddPolicy(CorsPolicyName, Policy =>
        {
            // Solo el origen del desktop; en producción se restringe al dominio real
            var AllowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:5173" };

            Policy.WithOrigins(AllowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }));

        return Services;
    }

    // Un método por módulo: aquí se irán sumando AddCareersModule, AddSubjectsModule, etc.
    public static IServiceCollection AddUSystemModules(
        this IServiceCollection Services,
        IConfiguration Configuration)
    {
        Services.AddApplication();
        Services.AddInfrastructure(Configuration);
        Services.AddSecurityModule(Configuration);
        Services.AddHealthModule();
        Services.AddUsersModule();
        Services.AddRolesModule();
        Services.AddAuditLogsModule();
        Services.AddSettingsModule();
        Services.AddCareersModule();
        Services.AddSubjectsModule();
        Services.AddAcademicTermsModule();
        Services.AddStudyPlansModule();
        return Services;
    }
}
