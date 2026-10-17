// Chequeo de salud de la API. No requiere token ni base de datos.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Api.Contracts;
using USystem.Application.Common;

namespace USystem.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly IHostEnvironment _Environment;

    public HealthController(IHostEnvironment Environment) => _Environment = Environment;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<HealthResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HealthResponse>> Get()
        => Ok(ApiResponse<HealthResponse>.Ok(new HealthResponse
        {
            Status = "Healthy",
            Environment = _Environment.EnvironmentName,
            Version = typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "0.0.0",
            Timestamp = DateTime.UtcNow,
        }));
}
