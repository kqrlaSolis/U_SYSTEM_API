// Registro de dependencias de infraestructura: EF Core, repositorios base, seguridad y almacenamiento.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Interfaces;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;
using USystem.Infrastructure.Repositories;
using USystem.Infrastructure.Security;
using USystem.Infrastructure.Storage;

namespace USystem.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection Services,
        IConfiguration Configuration)
    {
        var ConnectionString = Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            // La API arranca para responder /health, pero cualquier consulta falla con un error explícito
            ConnectionString = "Server=localhost;Port=3306;Database=USystem;User=USystemApp;Password=CHANGE_ME;";
        }

        var ServerVersion = MySqlVersionResolver.Resolve(Configuration);

        Services.AddDbContext<USystemDbContext>(Options =>
            Options.UseMySql(
                ConnectionString,
                ServerVersion,
                MySql => MySql.EnableRetryOnFailure(3)));

        Services.AddMemoryCache();
        Services.AddScoped<ISettingsProvider, SettingsProvider>();
        Services.AddScoped<IUnitOfWork, UnitOfWork>();
        Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        Services.AddScoped<AdminUserSeeder>();

        Services.Configure<JwtOptions>(Configuration.GetSection(JwtOptions.SectionName));
        Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        Services.Configure<StorageOptions>(Configuration.GetSection(StorageOptions.SectionName));
        Services.PostConfigure<StorageOptions>(Options =>
        {
            // Las credenciales y el bucket también pueden venir de las variables estándar de AWS
            if (string.IsNullOrWhiteSpace(Options.BucketName))
            {
                Options.BucketName = Configuration["AWS_BUCKET_NAME"] ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(Options.Region))
            {
                Options.Region = Configuration["AWS_REGION"] ?? "us-east-2";
            }

            if (string.IsNullOrWhiteSpace(Options.ServiceUrl))
            {
                Options.ServiceUrl = Configuration["AWS_ENDPOINT_URL"] ?? string.Empty;
            }
        });

        // El cliente de S3 es seguro entre hilos y caro de crear: se registra como singleton
        Services.AddSingleton<IFileStorage, S3FileStorage>();

        return Services;
    }
}
