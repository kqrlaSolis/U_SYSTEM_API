// Tipos de trámite y solicitudes.
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
[Route("api/v1/procedure-types")]
public class ProcedureTypesController : ControllerBase
{
    private readonly IProcedureService _ProcedureService;

    public ProcedureTypesController(IProcedureService ProcedureService)
        => _ProcedureService = ProcedureService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Procedures.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProcedureTypeDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<ProcedureTypeDto>>.Ok(await _ProcedureService.GetTypesAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Procedures.View)]
    public async Task<ActionResult<ApiResponse<ProcedureTypeDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<ProcedureTypeDto>.Ok(await _ProcedureService.GetTypeAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Procedures.Issue)]
    public async Task<ActionResult<ApiResponse<ProcedureTypeDto>>> Create(
        [FromBody] CreateProcedureTypeRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _ProcedureService.CreateTypeAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<ProcedureTypeDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Procedures.Issue)]
    public async Task<ActionResult<ApiResponse<ProcedureTypeDto>>> Update(
        int Id,
        [FromBody] UpdateProcedureTypeRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<ProcedureTypeDto>.Ok(await _ProcedureService.UpdateTypeAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Procedures.Issue)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _ProcedureService.DeleteTypeAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
