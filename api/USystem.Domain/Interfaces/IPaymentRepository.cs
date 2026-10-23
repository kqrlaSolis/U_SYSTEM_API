// Acceso a datos de conceptos, planes y cargos de pago.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<PagedResult<PaymentConcept>> GetConceptsPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<PaymentConcept?> GetConceptByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<bool> ExistsConceptByCodeAsync(
        string Code,
        int? ExcludeConceptId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    Task AddConceptAsync(PaymentConcept Entity, CancellationToken Cancellation = default);

    void UpdateConcept(PaymentConcept Entity);

    void RemoveConcept(PaymentConcept Entity);

    Task<PagedResult<PaymentPlan>> GetPlansPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<PaymentPlan?> GetPlanByIdAsync(int Id, CancellationToken Cancellation = default);

    Task AddPlanAsync(PaymentPlan Entity, CancellationToken Cancellation = default);

    void UpdatePlan(PaymentPlan Entity);

    void RemovePlan(PaymentPlan Entity);

    Task<PagedResult<Charge>> GetChargesPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<Charge?> GetChargeByIdAsync(long Id, CancellationToken Cancellation = default);

    Task AddChargeAsync(Charge Entity, CancellationToken Cancellation = default);

    void UpdateCharge(Charge Entity);
}
