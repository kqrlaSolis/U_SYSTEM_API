// Catálogo de conceptos de pago.
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
[Route("api/v1/payment-concepts")]
public class PaymentConceptsController : ControllerBase
{
    private readonly IPaymentService _PaymentService;

    public PaymentConceptsController(IPaymentService PaymentService) => _PaymentService = PaymentService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Payments.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentConceptDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<PaymentConceptDto>>.Ok(await _PaymentService.GetConceptsAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Payments.View)]
    public async Task<ActionResult<ApiResponse<PaymentConceptDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<PaymentConceptDto>.Ok(await _PaymentService.GetConceptAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Payments.Register)]
    public async Task<ActionResult<ApiResponse<PaymentConceptDto>>> Create(
        [FromBody] CreatePaymentConceptRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _PaymentService.CreateConceptAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<PaymentConceptDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Payments.Register)]
    public async Task<ActionResult<ApiResponse<PaymentConceptDto>>> Update(
        int Id,
        [FromBody] UpdatePaymentConceptRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PaymentConceptDto>.Ok(await _PaymentService.UpdateConceptAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Payments.Register)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _PaymentService.DeleteConceptAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
