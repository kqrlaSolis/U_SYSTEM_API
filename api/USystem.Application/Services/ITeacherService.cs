// Contratos del servicio de docentes.
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface ITeacherService
{
    Task<PagedResult<TeacherDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<TeacherDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<TeacherDto> CreateAsync(CreateTeacherRequest Request, CancellationToken Cancellation = default);

    Task<TeacherDto> UpdateAsync(int Id, UpdateTeacherRequest Request, CancellationToken Cancellation = default);

    Task DeleteAsync(int Id, CancellationToken Cancellation = default);
}
