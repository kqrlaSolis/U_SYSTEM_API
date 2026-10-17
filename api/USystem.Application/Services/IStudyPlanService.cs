// Casos de uso de los planes de estudio: versiones, malla, seriación y plan vigente.
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface IStudyPlanService
{
    Task<PagedResult<StudyPlanDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<StudyPlanDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<StudyPlanDto> CreateAsync(
        CreateStudyPlanRequest Request,
        CancellationToken Cancellation = default);

    Task<StudyPlanDto> UpdateAsync(
        int Id,
        UpdateStudyPlanRequest Request,
        CancellationToken Cancellation = default);

    Task DeleteAsync(int Id, CancellationToken Cancellation = default);

    Task<StudyPlanDto> ReplaceSubjectsAsync(
        int Id,
        StudyPlanSubjectsRequest Request,
        CancellationToken Cancellation = default);

    Task<StudyPlanDto> ReplacePrerequisitesAsync(
        int Id,
        StudyPlanPrerequisitesRequest Request,
        CancellationToken Cancellation = default);

    Task<StudyPlanDto> SetCurrentAsync(int Id, CancellationToken Cancellation = default);
}
