// Sesión del usuario autenticado: el desktop lo usa para validar el token y leer los permisos.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Api.Contracts;
using USystem.Application.Common;
using USystem.Domain.Interfaces;

namespace USystem.Api.Controllers;

[ApiController]
[Route("api/v1/session")]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly ICurrentUser _CurrentUser;

    public SessionController(ICurrentUser CurrentUser) => _CurrentUser = CurrentUser;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<SessionResponse>> Get()
        => Ok(ApiResponse<SessionResponse>.Ok(new SessionResponse
        {
            UserId = _CurrentUser.UserId ?? 0,
            Username = _CurrentUser.Username ?? string.Empty,
            Permissions = _CurrentUser.Permissions.ToArray(),
            IsAuthenticated = _CurrentUser.IsAuthenticated,
        }));
}
