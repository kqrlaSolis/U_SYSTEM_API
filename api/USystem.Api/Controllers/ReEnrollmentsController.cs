// Propuesta de reinscripción (materias elegibles).
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
[Route("api/v1/students/{StudentId:long}/re-enrollment-proposal")]
public class ReEnrollmentsController : ControllerBase
{
    private readonly IReEnrollmentService _ReEnrollmentService;
    private readonly IEnrollmentService _EnrollmentService;

    public ReEnrollmentsController(
        IReEnrollmentService ReEnrollmentService,
        IEnrollmentService EnrollmentService)
    {
        _ReEnrollmentService = ReEnrollmentService;
        _EnrollmentService = EnrollmentService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Enrollments.View)]
    public async Task<ActionResult<ApiResponse<ReEnrollmentProposalDto>>> Get(
        long StudentId,
        CancellationToken Cancellation)
        => Ok(ApiResponse<ReEnrollmentProposalDto>.Ok(await _ReEnrollmentService.ProposeAsync(StudentId, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Enrollments.Create)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Create(
        long StudentId,
        [FromBody] CreateEnrollmentRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<EnrollmentDto>.Ok(await _EnrollmentService.CreateReEnrollmentAsync(Request, Cancellation)));
}
