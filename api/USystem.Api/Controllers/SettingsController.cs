// Parámetros del sistema: listado y edición de valores.
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
[Route("api/v1/settings")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _SettingsService;

    public SettingsController(ISettingsService SettingsService) => _SettingsService = SettingsService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Settings.View)]
    public async Task<ActionResult<ApiResponse<List<SystemSettingDto>>>> GetAll(CancellationToken Cancellation)
    {
        var Settings = await _SettingsService.GetAllAsync(Cancellation);
        return Ok(ApiResponse<List<SystemSettingDto>>.Ok(Settings.ToList()));
    }

    [HttpPut("{Key}")]
    [RequirePermission(PermissionCodes.Settings.Edit)]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> Update(
        string Key,
        [FromBody] UpdateSettingRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<SystemSettingDto>.Ok(
            await _SettingsService.UpdateAsync(Key, Request, Cancellation)));
}
