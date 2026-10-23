// Envío de notificaciones a partir de una plantilla.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Api.Attributes;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Services;
using USystem.Domain.Constants;

namespace USystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _NotificationService;

    public NotificationsController(INotificationService NotificationService)
        => _NotificationService = NotificationService;

    [HttpPost]
    [RequirePermission(PermissionCodes.Notifications.Send)]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> Send(
        [FromBody] SendNotificationRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<NotificationDto>.Ok(await _NotificationService.SendAsync(Request, Cancellation)));
}
