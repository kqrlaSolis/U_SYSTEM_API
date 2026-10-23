// Kardex del alumno.
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
[Route("api/v1/students/{StudentId:long}/transcript")]
public class TranscriptsController : ControllerBase
{
    private readonly ITranscriptService _TranscriptService;

    public TranscriptsController(ITranscriptService TranscriptService)
        => _TranscriptService = TranscriptService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Grades.View)]
    public async Task<ActionResult<ApiResponse<TranscriptDto>>> Get(long StudentId, CancellationToken Cancellation)
        => Ok(ApiResponse<TranscriptDto>.Ok(await _TranscriptService.GetByStudentAsync(StudentId, Cancellation)));
}
