// CRUD del material didáctico.
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
[Route("api/v1/course-materials")]
public class CourseMaterialsController : ControllerBase
{
    private readonly ICourseMaterialService _MaterialService;

    public CourseMaterialsController(ICourseMaterialService MaterialService)
        => _MaterialService = MaterialService;

    [HttpGet]
    [RequirePermission(PermissionCodes.CourseMaterials.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<CourseMaterialDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<CourseMaterialDto>>.Ok(await _MaterialService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.CourseMaterials.View)]
    public async Task<ActionResult<ApiResponse<CourseMaterialDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<CourseMaterialDto>.Ok(await _MaterialService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.CourseMaterials.Create)]
    public async Task<ActionResult<ApiResponse<CourseMaterialDto>>> Create(
        [FromBody] CreateCourseMaterialRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _MaterialService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<CourseMaterialDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.CourseMaterials.Edit)]
    public async Task<ActionResult<ApiResponse<CourseMaterialDto>>> Update(
        int Id,
        [FromBody] UpdateCourseMaterialRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<CourseMaterialDto>.Ok(await _MaterialService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.CourseMaterials.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _MaterialService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
