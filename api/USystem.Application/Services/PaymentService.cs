// Catálogo de conceptos de pago con clave única y auditoría.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IPaymentService
{
    Task<PagedResult<PaymentConceptDto>> GetConceptsAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<PaymentConceptDto> GetConceptAsync(int Id, CancellationToken Cancellation = default);

    Task<PaymentConceptDto> CreateConceptAsync(CreatePaymentConceptRequest Request, CancellationToken Cancellation = default);

    Task<PaymentConceptDto> UpdateConceptAsync(int Id, UpdatePaymentConceptRequest Request, CancellationToken Cancellation = default);

    Task DeleteConceptAsync(int Id, CancellationToken Cancellation = default);

    Task<PagedResult<PaymentPlanDto>> GetPlansAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<PaymentPlanDto> GetPlanAsync(int Id, CancellationToken Cancellation = default);

    Task<PaymentPlanDto> CreatePlanAsync(PaymentPlanRequest Request, CancellationToken Cancellation = default);

    Task<PaymentPlanDto> UpdatePlanAsync(int Id, PaymentPlanRequest Request, CancellationToken Cancellation = default);

    Task DeletePlanAsync(int Id, CancellationToken Cancellation = default);

    Task<PagedResult<ChargeDto>> GetChargesAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<ChargeDto> GetChargeAsync(long Id, CancellationToken Cancellation = default);

    Task<ChargeDto> CreateChargeAsync(ChargeRequest Request, CancellationToken Cancellation = default);

    Task<ChargeDto> CancelChargeAsync(long Id, CancellationToken Cancellation = default);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _PaymentRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public PaymentService(
        IPaymentRepository PaymentRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _PaymentRepository = PaymentRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<PaymentConceptDto>> GetConceptsAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _PaymentRepository.GetConceptsPagedAsync(Query, Cancellation);
        return PagedResult<PaymentConceptDto>.Create(
            Page.Items.Select(ToDto).ToList(),
            Page.TotalCount,
            Query);
    }

    public async Task<PaymentConceptDto> GetConceptAsync(int Id, CancellationToken Cancellation = default)
    {
        var Concept = await _PaymentRepository.GetConceptByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.ConceptNotFound");

        return ToDto(Concept);
    }

    public async Task<PaymentConceptDto> CreateConceptAsync(
        CreatePaymentConceptRequest Request,
        CancellationToken Cancellation = default)
    {
        var Code = Request.Code.Trim().ToUpperInvariant();

        if (await _PaymentRepository.ExistsConceptByCodeAsync(Code, null, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Payments.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var NewConcept = new PaymentConcept
        {
            Code = Code,
            Name = Request.Name.Trim(),
            BaseAmount = Request.BaseAmount,
            IsRecurring = Request.IsRecurring,
            IsActive = true,
        };

        await _PaymentRepository.AddConceptAsync(NewConcept, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Payments",
            nameof(PaymentConcept),
            NewConcept.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewConcept.Code, NewConcept.Name, NewConcept.BaseAmount },
            Cancellation);

        return ToDto(NewConcept);
    }

    public async Task<PaymentConceptDto> UpdateConceptAsync(
        int Id,
        UpdatePaymentConceptRequest Request,
        CancellationToken Cancellation = default)
    {
        var Concept = await _PaymentRepository.GetConceptByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.ConceptNotFound");

        var Code = Request.Code.Trim().ToUpperInvariant();
        if (await _PaymentRepository.ExistsConceptByCodeAsync(Code, Concept.Id, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Payments.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var Before = new { Concept.Code, Concept.Name, Concept.BaseAmount, Concept.IsActive };

        Concept.Code = Code;
        Concept.Name = Request.Name.Trim();
        Concept.BaseAmount = Request.BaseAmount;
        Concept.IsRecurring = Request.IsRecurring;
        Concept.IsActive = Request.IsActive;

        _PaymentRepository.UpdateConcept(Concept);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Payments",
            nameof(PaymentConcept),
            Concept.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Concept.Code, Concept.Name, Concept.BaseAmount, Concept.IsActive },
            Cancellation);

        return ToDto(Concept);
    }

    public async Task DeleteConceptAsync(int Id, CancellationToken Cancellation = default)
    {
        var Concept = await _PaymentRepository.GetConceptByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.ConceptNotFound");

        _PaymentRepository.RemoveConcept(Concept);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Payments",
            nameof(PaymentConcept),
            Concept.Id.ToString(CultureInfo.InvariantCulture),
            new { Concept.Code, Concept.Name },
            null,
            Cancellation);
    }

    public async Task<PagedResult<PaymentPlanDto>> GetPlansAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _PaymentRepository.GetPlansPagedAsync(Query, Cancellation);
        return PagedResult<PaymentPlanDto>.Create(
            Page.Items.Select(ToPlanDto).ToList(),
            Page.TotalCount,
            Query);
    }

    public async Task<PaymentPlanDto> GetPlanAsync(int Id, CancellationToken Cancellation = default)
    {
        var Plan = await _PaymentRepository.GetPlanByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.PlanNotFound");

        return ToPlanDto(Plan);
    }

    public async Task<PaymentPlanDto> CreatePlanAsync(
        PaymentPlanRequest Request,
        CancellationToken Cancellation = default)
    {
        var NewPlan = new PaymentPlan
        {
            CareerId = Request.CareerId,
            AcademicTermId = Request.AcademicTermId,
            Name = Request.Name.Trim(),
            InstallmentCount = Request.InstallmentCount,
            IsActive = true,
        };

        await _PaymentRepository.AddPlanAsync(NewPlan, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Payments",
            nameof(PaymentPlan),
            NewPlan.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewPlan.Name, NewPlan.AcademicTermId, NewPlan.InstallmentCount },
            Cancellation);

        return ToPlanDto(NewPlan);
    }

    public async Task<PaymentPlanDto> UpdatePlanAsync(
        int Id,
        PaymentPlanRequest Request,
        CancellationToken Cancellation = default)
    {
        var Plan = await _PaymentRepository.GetPlanByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.PlanNotFound");

        var Before = new { Plan.Name, Plan.IsActive, Plan.InstallmentCount };

        Plan.CareerId = Request.CareerId;
        Plan.AcademicTermId = Request.AcademicTermId;
        Plan.Name = Request.Name.Trim();
        Plan.InstallmentCount = Request.InstallmentCount;
        Plan.IsActive = Request.IsActive;

        _PaymentRepository.UpdatePlan(Plan);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Payments",
            nameof(PaymentPlan),
            Plan.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Plan.Name, Plan.IsActive, Plan.InstallmentCount },
            Cancellation);

        return ToPlanDto(Plan);
    }

    public async Task DeletePlanAsync(int Id, CancellationToken Cancellation = default)
    {
        var Plan = await _PaymentRepository.GetPlanByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.PlanNotFound");

        _PaymentRepository.RemovePlan(Plan);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Payments",
            nameof(PaymentPlan),
            Plan.Id.ToString(CultureInfo.InvariantCulture),
            new { Plan.Name },
            null,
            Cancellation);
    }

    public async Task<PagedResult<ChargeDto>> GetChargesAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _PaymentRepository.GetChargesPagedAsync(Query, Cancellation);
        return PagedResult<ChargeDto>.Create(
            Page.Items.Select(ToChargeDto).ToList(),
            Page.TotalCount,
            Query);
    }

    public async Task<ChargeDto> GetChargeAsync(long Id, CancellationToken Cancellation = default)
    {
        var Charge = await _PaymentRepository.GetChargeByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.ChargeNotFound");

        return ToChargeDto(Charge);
    }

    public async Task<ChargeDto> CreateChargeAsync(
        ChargeRequest Request,
        CancellationToken Cancellation = default)
    {
        var Concept = await _PaymentRepository.GetConceptByIdAsync(Request.PaymentConceptId, Cancellation)
            ?? throw new NotFoundException("Payments.ConceptNotFound");

        if (!Concept.IsActive)
        {
            throw BusinessRuleException.Conflict("Payments.ConceptInactive");
        }

        if (Request.Amount < 0 || Request.Discount < 0 || Request.Surcharge < 0)
        {
            throw BusinessRuleException.Conflict("Payments.InvalidAmount");
        }

        var NewCharge = new Charge
        {
            StudentId = Request.StudentId,
            AcademicTermId = Request.AcademicTermId,
            PaymentConceptId = Concept.Id,
            Amount = Request.Amount,
            Discount = Request.Discount,
            Surcharge = Request.Surcharge,
            DueDate = Request.DueDate,
            Status = "Pending",
        };

        await _PaymentRepository.AddChargeAsync(NewCharge, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Payments",
            nameof(Charge),
            NewCharge.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewCharge.StudentId, Concept.Code, NewCharge.Amount },
            Cancellation);

        return ToChargeDto(NewCharge);
    }

    public async Task<ChargeDto> CancelChargeAsync(long Id, CancellationToken Cancellation = default)
    {
        var Charge = await _PaymentRepository.GetChargeByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Payments.ChargeNotFound");

        if (string.Equals(Charge.Status, "Paid", StringComparison.OrdinalIgnoreCase))
        {
            throw BusinessRuleException.Conflict("Payments.ChargePaid");
        }

        var Before = new { Charge.Status };

        Charge.Status = "Cancelled";
        _PaymentRepository.UpdateCharge(Charge);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Cancel",
            "Payments",
            nameof(Charge),
            Charge.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Charge.Status },
            Cancellation);

        return ToChargeDto(Charge);
    }

    private static PaymentPlanDto ToPlanDto(PaymentPlan Plan)
        => new()
        {
            Id = Plan.Id,
            CareerId = Plan.CareerId,
            AcademicTermId = Plan.AcademicTermId,
            Name = Plan.Name,
            InstallmentCount = Plan.InstallmentCount,
            IsActive = Plan.IsActive,
        };

    private static ChargeDto ToChargeDto(Charge Charge)
        => new()
        {
            Id = Charge.Id,
            StudentId = Charge.StudentId,
            AcademicTermId = Charge.AcademicTermId,
            PaymentConceptId = Charge.PaymentConceptId,
            Amount = Charge.Amount,
            Discount = Charge.Discount,
            Surcharge = Charge.Surcharge,
            DueDate = Charge.DueDate,
            Status = Charge.Status,
        };

    private static PaymentConceptDto ToDto(PaymentConcept Concept)
        => new()
        {
            Id = Concept.Id,
            Code = Concept.Code,
            Name = Concept.Name,
            BaseAmount = Concept.BaseAmount,
            IsRecurring = Concept.IsRecurring,
            IsActive = Concept.IsActive,
        };
}
