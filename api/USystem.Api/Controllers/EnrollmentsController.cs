// Inscripciones de alumnos con asignación de grupos.
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
[Route("api/v1/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _EnrollmentService;

    public EnrollmentsController(IEnrollmentService EnrollmentService)
        => _EnrollmentService = EnrollmentService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Enrollments.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<EnrollmentDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<EnrollmentDto>>.Ok(await _EnrollmentService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:long}")]
    [RequirePermission(PermissionCodes.Enrollments.View)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> GetById(long Id, CancellationToken Cancellation)
        => Ok(ApiResponse<EnrollmentDto>.Ok(await _EnrollmentService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Enrollments.Create)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Create(
        [FromBody] CreateEnrollmentRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _EnrollmentService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<EnrollmentDto>.Ok(Created));
    }

    [HttpPost("{Id:long}/cancel")]
    [RequirePermission(PermissionCodes.Enrollments.Cancel)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Cancel(
        long Id,
        [FromBody] CancelEnrollmentRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<EnrollmentDto>.Ok(await _EnrollmentService.CancelAsync(Id, Request.Reason, Cancellation)));
}
