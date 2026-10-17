// CRUD del catálogo de carreras.
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
[Route("api/v1/careers")]
public class CareersController : ControllerBase
{
    private readonly ICareerService _CareerService;

    public CareersController(ICareerService CareerService) => _CareerService = CareerService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Careers.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<CareerDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<CareerDto>>.Ok(await _CareerService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Careers.View)]
    public async Task<ActionResult<ApiResponse<CareerDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<CareerDto>.Ok(await _CareerService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Careers.Create)]
    public async Task<ActionResult<ApiResponse<CareerDto>>> Create(
        [FromBody] CreateCareerRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _CareerService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<CareerDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Careers.Edit)]
    public async Task<ActionResult<ApiResponse<CareerDto>>> Update(
        int Id,
        [FromBody] UpdateCareerRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<CareerDto>.Ok(await _CareerService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Careers.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _CareerService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
