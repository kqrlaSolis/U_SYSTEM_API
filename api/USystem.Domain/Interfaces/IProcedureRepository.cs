// Acceso a datos de trámites y sus tipos.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IProcedureRepository
{
    Task<PagedResult<ProcedureType>> GetTypesPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<ProcedureType?> GetTypeByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<bool> ExistsTypeByCodeAsync(
        string Code,
        int? ExcludeTypeId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    Task AddTypeAsync(ProcedureType Entity, CancellationToken Cancellation = default);

    void UpdateType(ProcedureType Entity);

    void RemoveType(ProcedureType Entity);

    Task<PagedResult<Procedure>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<Procedure?> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    Task<string?> GetLastFolioAsync(string Prefix, CancellationToken Cancellation = default);

    Task AddAsync(Procedure Entity, CancellationToken Cancellation = default);

    void Update(Procedure Entity);
}
