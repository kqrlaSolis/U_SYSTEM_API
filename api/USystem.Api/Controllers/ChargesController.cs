// Cargos por alumno.
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
[Route("api/v1/charges")]
public class ChargesController : ControllerBase
{
    private readonly IPaymentService _PaymentService;

    public ChargesController(IPaymentService PaymentService) => _PaymentService = PaymentService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Payments.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<ChargeDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<ChargeDto>>.Ok(await _PaymentService.GetChargesAsync(Query, Cancellation)));

    [HttpGet("{Id:long}")]
    [RequirePermission(PermissionCodes.Payments.View)]
    public async Task<ActionResult<ApiResponse<ChargeDto>>> GetById(long Id, CancellationToken Cancellation)
        => Ok(ApiResponse<ChargeDto>.Ok(await _PaymentService.GetChargeAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Payments.Register)]
    public async Task<ActionResult<ApiResponse<ChargeDto>>> Create(
        [FromBody] ChargeRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _PaymentService.CreateChargeAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<ChargeDto>.Ok(Created));
    }

    [HttpPost("{Id:long}/cancel")]
    [RequirePermission(PermissionCodes.Payments.Cancel)]
    public async Task<ActionResult<ApiResponse<ChargeDto>>> Cancel(long Id, CancellationToken Cancellation)
        => Ok(ApiResponse<ChargeDto>.Ok(await _PaymentService.CancelChargeAsync(Id, Cancellation)));
}
