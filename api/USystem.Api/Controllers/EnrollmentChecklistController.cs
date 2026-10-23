// Expediente de inscripción: checklist de documentos y adeudos.
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
[Route("api/v1")]
public class EnrollmentChecklistController : ControllerBase
{
    private readonly IEnrollmentChecklistService _ChecklistService;
    private readonly IEnrollmentService _EnrollmentService;

    public EnrollmentChecklistController(
        IEnrollmentChecklistService ChecklistService,
        IEnrollmentService EnrollmentService)
    {
        _ChecklistService = ChecklistService;
        _EnrollmentService = EnrollmentService;
    }

    [HttpGet("students/{StudentId:long}/enrollment-checklist")]
    [RequirePermission(PermissionCodes.Enrollments.View)]
    public async Task<ActionResult<ApiResponse<EnrollmentChecklistDto>>> Get(
        long StudentId,
        [FromQuery] int AcademicTermId,
        CancellationToken Cancellation)
        => Ok(ApiResponse<EnrollmentChecklistDto>.Ok(
            await _ChecklistService.GetAsync(StudentId, AcademicTermId, Cancellation)));

    [HttpPost("students/{StudentId:long}/documents")]
    [RequirePermission(PermissionCodes.Enrollments.Create)]
    public async Task<ActionResult<ApiResponse<ChecklistDocumentDto>>> Register(
        long StudentId,
        [FromBody] RegisterStudentDocumentRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<ChecklistDocumentDto>.Ok(
            await _ChecklistService.RegisterDocumentAsync(StudentId, Request, Cancellation)));

    [HttpPost("student-documents/{DocumentId:long}/validate")]
    [RequirePermission(PermissionCodes.Enrollments.Create)]
    public async Task<ActionResult<ApiResponse<ChecklistDocumentDto>>> Validate(
        long DocumentId,
        CancellationToken Cancellation)
        => Ok(ApiResponse<ChecklistDocumentDto>.Ok(
            await _ChecklistService.ValidateDocumentAsync(DocumentId, Cancellation)));

    [HttpPost("enrollments/{EnrollmentId:long}/confirm")]
    [RequirePermission(PermissionCodes.Enrollments.Create)]
    public async Task<ActionResult<ApiResponse<bool>>> Confirm(
        long EnrollmentId,
        CancellationToken Cancellation)
    {
        await _ChecklistService.ConfirmAsync(EnrollmentId, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
