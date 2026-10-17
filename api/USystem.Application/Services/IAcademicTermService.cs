// Casos de uso de los periodos cuatrimestrales.
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface IAcademicTermService
{
    Task<PagedResult<AcademicTermDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default);

    Task<AcademicTermDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<AcademicTermDto> GetCurrentAsync(CancellationToken Cancellation = default);

    Task<AcademicTermDto> CreateAsync(
        CreateAcademicTermRequest Request,
        CancellationToken Cancellation = default);

    Task<AcademicTermDto> UpdateAsync(
        int Id,
        UpdateAcademicTermRequest Request,
        CancellationToken Cancellation = default);

    Task<AcademicTermDto> OpenAsync(int Id, CancellationToken Cancellation = default);

    Task<AcademicTermDto> CloseAsync(int Id, CancellationToken Cancellation = default);
}
