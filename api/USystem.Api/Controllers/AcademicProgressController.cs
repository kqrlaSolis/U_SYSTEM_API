// Avance académico del alumno.
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
[Route("api/v1/students/{StudentId:long}/academic-progress")]
public class AcademicProgressController : ControllerBase
{
    private readonly IAcademicProgressService _ProgressService;

    public AcademicProgressController(IAcademicProgressService ProgressService)
        => _ProgressService = ProgressService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Grades.View)]
    public async Task<ActionResult<ApiResponse<AcademicProgressDto>>> Get(
        long StudentId,
        CancellationToken Cancellation)
        => Ok(ApiResponse<AcademicProgressDto>.Ok(await _ProgressService.GetByStudentAsync(StudentId, Cancellation)));
}
