// Casos de uso del catálogo de carreras.
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface ICareerService
{
    Task<PagedResult<CareerDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<CareerDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<CareerDto> CreateAsync(CreateCareerRequest Request, CancellationToken Cancellation = default);

    Task<CareerDto> UpdateAsync(
        int Id,
        UpdateCareerRequest Request,
        CancellationToken Cancellation = default);

    Task DeleteAsync(int Id, CancellationToken Cancellation = default);
}
