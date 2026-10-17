// Módulo de seguridad: autenticación JWT y respuestas 401 uniformes.
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using USystem.Api.Common;
using USystem.Application.Common;
using USystem.Application.Interfaces;
using USystem.Infrastructure.Security;

namespace USystem.Api.Extensions;

public static class SecurityModuleExtensions
{
    public static IServiceCollection AddSecurityModule(
        this IServiceCollection Services,
        IConfiguration Configuration)
    {
        var Jwt = Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(Jwt.SecretKey) || Jwt.SecretKey.Length < JwtOptions.MinimumSecretLength)
        {
            throw new InvalidOperationException(
                "Configura Jwt:SecretKey (variable Jwt__SecretKey) con al menos "
                + $"{JwtOptions.MinimumSecretLength} caracteres antes de arrancar la API.");
        }

        Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = Jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };

                // Los 401 también salen con el sobre ApiResponse y el mensaje traducido
                Options.Events = new JwtBearerEvents
                {
                    OnChallenge = async ChallengeContext =>
                    {
                        ChallengeContext.HandleResponse();

                        if (ChallengeContext.Response.HasStarted)
                        {
                            return;
                        }

                        var Messages = ChallengeContext.HttpContext.RequestServices
                            .GetRequiredService<IErrorMessageProvider>();
                        var Language = Messages.ResolveLanguage(
                            ChallengeContext.Request.Headers.AcceptLanguage.ToString());
                        var Message = Messages.GetMessage("Api.Unauthenticated", null, Language);

                        var Response = ApiResponse<object>.Fail(
                            Message,
                            new ApiError { Code = "Api.Unauthenticated", Message = Message });

                        ChallengeContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await ChallengeContext.Response.WriteAsJsonAsync(
                            Response,
                            ApiJson.SerializerOptions,
                            "application/json",
                            ChallengeContext.HttpContext.RequestAborted);
                    },
                };
            });

        Services.AddAuthorization();
        return Services;
    }
}
