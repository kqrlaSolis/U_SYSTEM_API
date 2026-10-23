// Contratos del servicio de alumnos.
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface IStudentService
{
    Task<PagedResult<StudentDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<StudentDto> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    Task<StudentDto> CreateAsync(CreateStudentRequest Request, CancellationToken Cancellation = default);

    Task<StudentDto> UpdateAsync(long Id, UpdateStudentRequest Request, CancellationToken Cancellation = default);

    Task DeleteAsync(long Id, CancellationToken Cancellation = default);
}
