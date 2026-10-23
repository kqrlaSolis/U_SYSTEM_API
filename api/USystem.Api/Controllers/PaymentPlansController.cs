// Planes de pago.
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
[Route("api/v1/payment-plans")]
public class PaymentPlansController : ControllerBase
{
    private readonly IPaymentService _PaymentService;

    public PaymentPlansController(IPaymentService PaymentService) => _PaymentService = PaymentService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Payments.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentPlanDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<PaymentPlanDto>>.Ok(await _PaymentService.GetPlansAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Payments.View)]
    public async Task<ActionResult<ApiResponse<PaymentPlanDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<PaymentPlanDto>.Ok(await _PaymentService.GetPlanAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Payments.Register)]
    public async Task<ActionResult<ApiResponse<PaymentPlanDto>>> Create(
        [FromBody] PaymentPlanRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _PaymentService.CreatePlanAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<PaymentPlanDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Payments.Register)]
    public async Task<ActionResult<ApiResponse<PaymentPlanDto>>> Update(
        int Id,
        [FromBody] PaymentPlanRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PaymentPlanDto>.Ok(await _PaymentService.UpdatePlanAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Payments.Register)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _PaymentService.DeletePlanAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
