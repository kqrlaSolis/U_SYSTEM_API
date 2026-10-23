// Dashboard de indicadores.
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
[Route("api/v1/reports/dashboard")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _ReportService;

    public ReportsController(IReportService ReportService) => _ReportService = ReportService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Reports.View)]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> GetDashboard(CancellationToken Cancellation)
        => Ok(ApiResponse<DashboardDto>.Ok(await _ReportService.GetDashboardAsync(Cancellation)));
}
