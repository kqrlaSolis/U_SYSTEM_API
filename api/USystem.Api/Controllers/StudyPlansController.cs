// Planes de estudio: CRUD, malla curricular, seriación y plan vigente.
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
[Route("api/v1/study-plans")]
public class StudyPlansController : ControllerBase
{
    private readonly IStudyPlanService _StudyPlanService;

    public StudyPlansController(IStudyPlanService StudyPlanService) => _StudyPlanService = StudyPlanService;

    [HttpGet]
    [RequirePermission(PermissionCodes.StudyPlans.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<StudyPlanDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<StudyPlanDto>>.Ok(
            await _StudyPlanService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.StudyPlans.View)]
    public async Task<ActionResult<ApiResponse<StudyPlanDto>>> GetById(
        int Id,
        CancellationToken Cancellation)
        => Ok(ApiResponse<StudyPlanDto>.Ok(await _StudyPlanService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.StudyPlans.Create)]
    public async Task<ActionResult<ApiResponse<StudyPlanDto>>> Create(
        [FromBody] CreateStudyPlanRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _StudyPlanService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(
            nameof(GetById),
            new { Id = Created.Id },
            ApiResponse<StudyPlanDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.StudyPlans.Edit)]
    public async Task<ActionResult<ApiResponse<StudyPlanDto>>> Update(
        int Id,
        [FromBody] UpdateStudyPlanRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<StudyPlanDto>.Ok(await _StudyPlanService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.StudyPlans.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _StudyPlanService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }

    [HttpPut("{Id:int}/subjects")]
    [RequirePermission(PermissionCodes.StudyPlans.Edit)]
    public async Task<ActionResult<ApiResponse<StudyPlanDto>>> ReplaceSubjects(
        int Id,
        [FromBody] StudyPlanSubjectsRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<StudyPlanDto>.Ok(
            await _StudyPlanService.ReplaceSubjectsAsync(Id, Request, Cancellation)));

    [HttpPut("{Id:int}/prerequisites")]
    [RequirePermission(PermissionCodes.StudyPlans.Edit)]
    public async Task<ActionResult<ApiResponse<StudyPlanDto>>> ReplacePrerequisites(
        int Id,
        [FromBody] StudyPlanPrerequisitesRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<StudyPlanDto>.Ok(
            await _StudyPlanService.ReplacePrerequisitesAsync(Id, Request, Cancellation)));

    [HttpPost("{Id:int}/set-current")]
    [RequirePermission(PermissionCodes.StudyPlans.Edit)]
    public async Task<ActionResult<ApiResponse<StudyPlanDto>>> SetCurrent(
        int Id,
        CancellationToken Cancellation)
        => Ok(ApiResponse<StudyPlanDto>.Ok(await _StudyPlanService.SetCurrentAsync(Id, Cancellation)));
}
