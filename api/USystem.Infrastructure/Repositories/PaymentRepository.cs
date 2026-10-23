// Acceso a datos de conceptos de pago.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly USystemDbContext _Context;

    public PaymentRepository(USystemDbContext Context) => _Context = Context;

    public async Task<PagedResult<PaymentConcept>> GetConceptsPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.PaymentConcepts.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Concept => Concept.Code.Contains(Search) || Concept.Name.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Concept => Concept.Name)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<PaymentConcept>.Create(Items, TotalCount, Query);
    }

    public Task<PaymentConcept?> GetConceptByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.PaymentConcepts.FirstOrDefaultAsync(Concept => Concept.Id == Id, Cancellation);

    public Task<bool> ExistsConceptByCodeAsync(
        string Code,
        int? ExcludeConceptId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted
            ? _Context.PaymentConcepts.IgnoreQueryFilters()
            : _Context.PaymentConcepts;

        return Source.AnyAsync(
            Concept => Concept.Code == Code
                && (ExcludeConceptId == null || Concept.Id != ExcludeConceptId),
            Cancellation);
    }

    public async Task<PagedResult<PaymentPlan>> GetPlansPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.PaymentPlans.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Plan => Plan.Name.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Plan => Plan.Name)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<PaymentPlan>.Create(Items, TotalCount, Query);
    }

    public Task<PaymentPlan?> GetPlanByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.PaymentPlans.FirstOrDefaultAsync(Plan => Plan.Id == Id, Cancellation);

    public async Task AddPlanAsync(PaymentPlan Entity, CancellationToken Cancellation = default)
        => await _Context.PaymentPlans.AddAsync(Entity, Cancellation);

    public void UpdatePlan(PaymentPlan Entity) => _Context.PaymentPlans.Update(Entity);

    public void RemovePlan(PaymentPlan Entity) => _Context.PaymentPlans.Remove(Entity);

    public async Task<PagedResult<Charge>> GetChargesPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Charges.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Charge => Charge.Status.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderByDescending(Charge => Charge.Id)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Charge>.Create(Items, TotalCount, Query);
    }

    public Task<Charge?> GetChargeByIdAsync(long Id, CancellationToken Cancellation = default)
        => _Context.Charges.FirstOrDefaultAsync(Charge => Charge.Id == Id, Cancellation);

    public async Task AddChargeAsync(Charge Entity, CancellationToken Cancellation = default)
        => await _Context.Charges.AddAsync(Entity, Cancellation);

    public void UpdateCharge(Charge Entity) => _Context.Charges.Update(Entity);

    public async Task AddConceptAsync(PaymentConcept Entity, CancellationToken Cancellation = default)
        => await _Context.PaymentConcepts.AddAsync(Entity, Cancellation);

    public void UpdateConcept(PaymentConcept Entity) => _Context.PaymentConcepts.Update(Entity);

    public void RemoveConcept(PaymentConcept Entity) => _Context.PaymentConcepts.Remove(Entity);
}
