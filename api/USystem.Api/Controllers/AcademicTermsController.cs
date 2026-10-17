// Calendario cuatrimestral: CRUD, apertura, cierre y periodo en curso.
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
[Route("api/v1/academic-terms")]
public class AcademicTermsController : ControllerBase
{
    private readonly IAcademicTermService _TermService;

    public AcademicTermsController(IAcademicTermService TermService) => _TermService = TermService;

    [HttpGet]
    [RequirePermission(PermissionCodes.AcademicTerms.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<AcademicTermDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<AcademicTermDto>>.Ok(
            await _TermService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("current")]
    [RequirePermission(PermissionCodes.AcademicTerms.View)]
    public async Task<ActionResult<ApiResponse<AcademicTermDto>>> GetCurrent(CancellationToken Cancellation)
        => Ok(ApiResponse<AcademicTermDto>.Ok(await _TermService.GetCurrentAsync(Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.AcademicTerms.View)]
    public async Task<ActionResult<ApiResponse<AcademicTermDto>>> GetById(
        int Id,
        CancellationToken Cancellation)
        => Ok(ApiResponse<AcademicTermDto>.Ok(await _TermService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.AcademicTerms.Create)]
    public async Task<ActionResult<ApiResponse<AcademicTermDto>>> Create(
        [FromBody] CreateAcademicTermRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _TermService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(
            nameof(GetById),
            new { Id = Created.Id },
            ApiResponse<AcademicTermDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.AcademicTerms.Edit)]
    public async Task<ActionResult<ApiResponse<AcademicTermDto>>> Update(
        int Id,
        [FromBody] UpdateAcademicTermRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<AcademicTermDto>.Ok(await _TermService.UpdateAsync(Id, Request, Cancellation)));

    [HttpPost("{Id:int}/open")]
    [RequirePermission(PermissionCodes.AcademicTerms.Open)]
    public async Task<ActionResult<ApiResponse<AcademicTermDto>>> Open(
        int Id,
        CancellationToken Cancellation)
        => Ok(ApiResponse<AcademicTermDto>.Ok(await _TermService.OpenAsync(Id, Cancellation)));

    [HttpPost("{Id:int}/close")]
    [RequirePermission(PermissionCodes.AcademicTerms.Close)]
    public async Task<ActionResult<ApiResponse<AcademicTermDto>>> Close(
        int Id,
        CancellationToken Cancellation)
        => Ok(ApiResponse<AcademicTermDto>.Ok(await _TermService.CloseAsync(Id, Cancellation)));
}
