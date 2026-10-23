// CRUD de alumnos con generación de matrícula.
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
[Route("api/v1/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _StudentService;

    public StudentsController(IStudentService StudentService) => _StudentService = StudentService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Students.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<StudentDto>>.Ok(await _StudentService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:long}")]
    [RequirePermission(PermissionCodes.Students.View)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(long Id, CancellationToken Cancellation)
        => Ok(ApiResponse<StudentDto>.Ok(await _StudentService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Students.Create)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Create(
        [FromBody] CreateStudentRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _StudentService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<StudentDto>.Ok(Created));
    }

    [HttpPut("{Id:long}")]
    [RequirePermission(PermissionCodes.Students.Edit)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Update(
        long Id,
        [FromBody] UpdateStudentRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<StudentDto>.Ok(await _StudentService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:long}")]
    [RequirePermission(PermissionCodes.Students.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long Id, CancellationToken Cancellation)
    {
        await _StudentService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
