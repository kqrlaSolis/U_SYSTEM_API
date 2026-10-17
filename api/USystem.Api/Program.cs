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

        if (App.Environment.IsDevelopment())
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
