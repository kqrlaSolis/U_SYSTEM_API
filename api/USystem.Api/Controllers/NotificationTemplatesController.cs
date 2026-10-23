// Plantillas de notificación y envío.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Api.Attributes;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Services;
using USystem.Domain.Common;
using USystem.Domain.Constants;

namespace USystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/notification-templates")]
public class NotificationTemplatesController : ControllerBase
{
    private readonly INotificationService _NotificationService;

    public NotificationTemplatesController(INotificationService NotificationService)
        => _NotificationService = NotificationService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Notifications.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationTemplateDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<NotificationTemplateDto>>.Ok(await _NotificationService.GetTemplatesAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Notifications.View)]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<NotificationTemplateDto>.Ok(await _NotificationService.GetTemplateAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Notifications.Send)]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> Create(
        [FromBody] CreateNotificationTemplateRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _NotificationService.CreateTemplateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<NotificationTemplateDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Notifications.Send)]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> Update(
        int Id,
        [FromBody] UpdateNotificationTemplateRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<NotificationTemplateDto>.Ok(await _NotificationService.UpdateTemplateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Notifications.Send)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _NotificationService.DeleteTemplateAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
