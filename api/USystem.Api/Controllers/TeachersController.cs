// CRUD de docentes.
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
[Route("api/v1/teachers")]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _TeacherService;

    public TeachersController(ITeacherService TeacherService) => _TeacherService = TeacherService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Teachers.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<TeacherDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<TeacherDto>>.Ok(await _TeacherService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Teachers.View)]
    public async Task<ActionResult<ApiResponse<TeacherDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<TeacherDto>.Ok(await _TeacherService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Teachers.Create)]
    public async Task<ActionResult<ApiResponse<TeacherDto>>> Create(
        [FromBody] CreateTeacherRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _TeacherService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<TeacherDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Teachers.Edit)]
    public async Task<ActionResult<ApiResponse<TeacherDto>>> Update(
        int Id,
        [FromBody] UpdateTeacherRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<TeacherDto>.Ok(await _TeacherService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Teachers.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _TeacherService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
