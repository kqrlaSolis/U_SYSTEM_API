// Punto de entrada de la API: configuración, capas, middleware y rutas.
using Serilog;
using USystem.Api.Configuration;
using USystem.Api.Extensions;
using USystem.Api.Middlewares;
using USystem.Infrastructure.Persistence;

namespace USystem.Api;

public class Program
{
    public static void Main(string[] Args)
    {
        // .env no se versiona: se carga en la raíz del repositorio antes de leer la configuración
        EnvironmentFileLoader.Load();

        // Comando de una sola vez para crear el administrador inicial (no corre en el arranque normal)
        if (Args.Contains("--seed-admin", StringComparer.OrdinalIgnoreCase))
        {
            SeedAdminAsync().GetAwaiter().GetResult();
            return;
        }

        var Builder = WebApplication.CreateBuilder(Args);

        Builder.Host.UseSerilog((Context, LoggerConfiguration) =>
            LoggerConfiguration.ReadFrom.Configuration(Context.Configuration));

        Builder.Services.AddUSystemApi(Builder.Configuration);
        Builder.Services.AddUSystemModules(Builder.Configuration);

        var App = Builder.Build();

        App.UseSerilogRequestLogging();

        // Siempre primero: cualquier excepción posterior sale con el sobre ApiResponse
        App.UseMiddleware<ExceptionHandlingMiddleware>();

        // Swagger en desarrollo o cuando el despliegue lo habilita con Swagger__Enabled=true
        if (App.Environment.IsDevelopment() || App.Configuration.GetValue<bool>("Swagger:Enabled"))
        {
            App.UseSwagger();
            App.UseSwaggerUI(Options =>
                Options.SwaggerEndpoint("/swagger/v1/swagger.json", "U SYSTEM API v1"));
        }

        App.UseCors(ServiceCollectionExtensions.CorsPolicyName);
        App.UseAuthentication();
        App.UseAuthorization();
        App.MapControllers();
        App.MapHealthChecks("/health");

        // Raíz informativa: abrir el dominio del servicio no debe devolver 404
        App.MapGet("/", () => Results.Json(new
        {
            Name = "U SYSTEM API",
            Environment = App.Environment.EnvironmentName,
            Version = App.GetType().Assembly.GetName().Version?.ToString() ?? "0.0.0",
            Health = "/health",
            Swagger = App.Configuration.GetValue<bool>("Swagger:Enabled") ? "/swagger" : "habilitar con Swagger__Enabled=true",
        })).AllowAnonymous();

        App.Run();
    }

    // `dotnet run --project api/USystem.Api -- --seed-admin` con Seed__AdminPassword en el entorno
    private static async Task SeedAdminAsync()
    {
        var Builder = WebApplication.CreateBuilder(Array.Empty<string>());

        Builder.Host.UseSerilog((Context, LoggerConfiguration) =>
            LoggerConfiguration.ReadFrom.Configuration(Context.Configuration));

        Builder.Services.AddUSystemApi(Builder.Configuration);
        Builder.Services.AddUSystemModules(Builder.Configuration);

        await using var App = Builder.Build();

        var Password = Environment.GetEnvironmentVariable("Seed__AdminPassword");
        if (string.IsNullOrWhiteSpace(Password))
        {
            throw new InvalidOperationException(
                "Define Seed__AdminPassword (8+ caracteres con mayúscula, minúscula, número y símbolo).");
        }

        using var Scope = App.Services.CreateScope();
        var Seeder = Scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
        var UserId = await Seeder.SeedAsync(Password);

        Console.WriteLine($"Administrador listo (Id {UserId}).");
    }
}
