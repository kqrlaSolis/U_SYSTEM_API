// Solicitudes de trámite con folio.
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
[Route("api/v1/procedures")]
public class ProceduresController : ControllerBase
{
    private readonly IProcedureService _ProcedureService;

    public ProceduresController(IProcedureService ProcedureService)
        => _ProcedureService = ProcedureService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Procedures.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProcedureDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<ProcedureDto>>.Ok(await _ProcedureService.GetProceduresAsync(Query, Cancellation)));

    [HttpGet("{Id:long}")]
    [RequirePermission(PermissionCodes.Procedures.View)]
    public async Task<ActionResult<ApiResponse<ProcedureDto>>> GetById(long Id, CancellationToken Cancellation)
        => Ok(ApiResponse<ProcedureDto>.Ok(await _ProcedureService.GetProcedureAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Procedures.Request)]
    public async Task<ActionResult<ApiResponse<ProcedureDto>>> Create(
        [FromBody] CreateProcedureRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _ProcedureService.CreateProcedureAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<ProcedureDto>.Ok(Created));
    }

    [HttpPost("{Id:long}/issue")]
    [RequirePermission(PermissionCodes.Procedures.Issue)]
    public async Task<ActionResult<ApiResponse<ProcedureDto>>> Issue(
        long Id,
        [FromBody] IssueProcedureRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<ProcedureDto>.Ok(await _ProcedureService.IssueAsync(Id, Request, Cancellation)));
}
