// Captura de calificaciones por grupo.
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
[Route("api/v1/class-groups/{ClassGroupId:int}/grades")]
public class GradesController : ControllerBase
{
    private readonly IGradeService _GradeService;

    public GradesController(IGradeService GradeService) => _GradeService = GradeService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Grades.View)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GradeDto>>>> Get(
        int ClassGroupId,
        CancellationToken Cancellation)
        => Ok(ApiResponse<IReadOnlyList<GradeDto>>.Ok(await _GradeService.GetByClassGroupAsync(ClassGroupId, Cancellation)));

    [HttpPut]
    [RequirePermission(PermissionCodes.Grades.Capture)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GradeDto>>>> Save(
        int ClassGroupId,
        [FromBody] SaveGradesRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<IReadOnlyList<GradeDto>>.Ok(await _GradeService.SaveAsync(ClassGroupId, Request, Cancellation)));
}
