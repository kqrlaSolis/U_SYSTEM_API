// CRUD del catálogo de materias.
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
[Route("api/v1/subjects")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _SubjectService;

    public SubjectsController(ISubjectService SubjectService) => _SubjectService = SubjectService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Subjects.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<SubjectDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<SubjectDto>>.Ok(await _SubjectService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Subjects.View)]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<SubjectDto>.Ok(await _SubjectService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Subjects.Create)]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> Create(
        [FromBody] CreateSubjectRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _SubjectService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<SubjectDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Subjects.Edit)]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> Update(
        int Id,
        [FromBody] UpdateSubjectRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<SubjectDto>.Ok(await _SubjectService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Subjects.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _SubjectService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
