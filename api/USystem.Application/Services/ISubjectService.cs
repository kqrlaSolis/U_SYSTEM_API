// Casos de uso del catálogo de materias.
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface ISubjectService
{
    Task<PagedResult<SubjectDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<SubjectDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<SubjectDto> CreateAsync(CreateSubjectRequest Request, CancellationToken Cancellation = default);

    Task<SubjectDto> UpdateAsync(
        int Id,
        UpdateSubjectRequest Request,
        CancellationToken Cancellation = default);

    Task DeleteAsync(int Id, CancellationToken Cancellation = default);
}
