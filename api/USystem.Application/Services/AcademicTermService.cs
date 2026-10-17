// Reglas del calendario: sin traslapes, un solo periodo en curso y cierre de solo lectura.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Constants;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class AcademicTermService : IAcademicTermService
{
    private readonly IAcademicTermRepository _TermRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public AcademicTermService(
        IAcademicTermRepository TermRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _TermRepository = TermRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<AcademicTermDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _TermRepository.GetPagedAsync(Query, Cancellation);
        var Items = Page.Items.Select(CatalogMapper.ToDto).ToList();
        return PagedResult<AcademicTermDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<AcademicTermDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Term = await _TermRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("AcademicTerms.NotFound");

        return CatalogMapper.ToDto(Term);
    }

    public async Task<AcademicTermDto> GetCurrentAsync(CancellationToken Cancellation = default)
    {
        var Term = await _TermRepository.GetCurrentAsync(Cancellation)
            ?? throw new NotFoundException("AcademicTerms.NotFound");

        return CatalogMapper.ToDto(Term);
    }

    public async Task<AcademicTermDto> CreateAsync(
        CreateAcademicTermRequest Request,
        CancellationToken Cancellation = default)
    {
        var Code = Request.Code.Trim();
        await EnsureUniqueCodeAsync(Code, null, Cancellation);
        await EnsureNoOverlapAsync(Request.StartDate, Request.EndDate, null, Cancellation);

        var NewTerm = new AcademicTerm
        {
            Code = Code,
            Name = Request.Name.Trim(),
            AcademicYear = Request.AcademicYear,
            TermNumber = Request.TermNumber,
            StartDate = Request.StartDate,
            EndDate = Request.EndDate,
            EnrollmentStartDate = Request.EnrollmentStartDate,
            EnrollmentEndDate = Request.EnrollmentEndDate,
            ReEnrollmentStartDate = Request.ReEnrollmentStartDate,
            ReEnrollmentEndDate = Request.ReEnrollmentEndDate,
            ExamStartDate = Request.ExamStartDate,
            ExamEndDate = Request.ExamEndDate,
            GradeReportCloseDate = Request.GradeReportCloseDate,
            Status = AcademicTermStatuses.Planned,
        };

        await _TermRepository.AddAsync(NewTerm, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "AcademicTerms",
            nameof(AcademicTerm),
            NewTerm.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewTerm.Code, NewTerm.StartDate, NewTerm.EndDate },
            Cancellation);

        return CatalogMapper.ToDto(NewTerm);
    }

    public async Task<AcademicTermDto> UpdateAsync(
        int Id,
        UpdateAcademicTermRequest Request,
        CancellationToken Cancellation = default)
    {
        var Term = await _TermRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("AcademicTerms.NotFound");

        // Un periodo cerrado es de solo lectura
        if (Term.Status == AcademicTermStatuses.Closed)
        {
            throw new BusinessRuleException(
                "AcademicTerms.Closed",
                new Dictionary<string, string> { ["Code"] = Term.Code });
        }

        var Code = Request.Code.Trim();
        await EnsureUniqueCodeAsync(Code, Term.Id, Cancellation);
        await EnsureNoOverlapAsync(Request.StartDate, Request.EndDate, Term.Id, Cancellation);

        var Before = new { Term.Code, Term.Name, Term.StartDate, Term.EndDate, Term.Status };

        Term.Code = Code;
        Term.Name = Request.Name.Trim();
        Term.AcademicYear = Request.AcademicYear;
        Term.TermNumber = Request.TermNumber;
        Term.StartDate = Request.StartDate;
        Term.EndDate = Request.EndDate;
        Term.EnrollmentStartDate = Request.EnrollmentStartDate;
        Term.EnrollmentEndDate = Request.EnrollmentEndDate;
        Term.ReEnrollmentStartDate = Request.ReEnrollmentStartDate;
        Term.ReEnrollmentEndDate = Request.ReEnrollmentEndDate;
        Term.ExamStartDate = Request.ExamStartDate;
        Term.ExamEndDate = Request.ExamEndDate;
        Term.GradeReportCloseDate = Request.GradeReportCloseDate;

        _TermRepository.Update(Term);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "AcademicTerms",
            nameof(AcademicTerm),
            Term.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Term.Code, Term.Name, Term.StartDate, Term.EndDate, Term.Status },
            Cancellation);

        return CatalogMapper.ToDto(Term);
    }

    public async Task<AcademicTermDto> OpenAsync(int Id, CancellationToken Cancellation = default)
    {
        var Term = await _TermRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("AcademicTerms.NotFound");

        if (Term.Status == AcademicTermStatuses.Closed)
        {
            throw new BusinessRuleException(
                "AcademicTerms.Closed",
                new Dictionary<string, string> { ["Code"] = Term.Code });
        }

        if (Term.Status != AcademicTermStatuses.Planned)
        {
            throw BusinessRuleException.Conflict(
                "AcademicTerms.InvalidTransition",
                new Dictionary<string, string> { ["Code"] = Term.Code });
        }

        // Solo puede haber un periodo abierto o en curso a la vez
        var Active = await _TermRepository.GetActiveAsync(Cancellation);
        if (Active is not null && Active.Id != Term.Id)
        {
            throw new BusinessRuleException(
                "AcademicTerms.OnlyOneInProgress",
                new Dictionary<string, string> { ["Code"] = Active.Code });
        }

        Term.Status = AcademicTermStatuses.Open;
        _TermRepository.Update(Term);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Open",
            "AcademicTerms",
            nameof(AcademicTerm),
            Term.Id.ToString(CultureInfo.InvariantCulture),
            new { Status = AcademicTermStatuses.Planned },
            new { Status = AcademicTermStatuses.Open },
            Cancellation);

        return CatalogMapper.ToDto(Term);
    }

    public async Task<AcademicTermDto> CloseAsync(int Id, CancellationToken Cancellation = default)
    {
        var Term = await _TermRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("AcademicTerms.NotFound");

        if (Term.Status == AcademicTermStatuses.Closed)
        {
            throw BusinessRuleException.Conflict(
                "AcademicTerms.InvalidTransition",
                new Dictionary<string, string> { ["Code"] = Term.Code });
        }

        var PreviousStatus = Term.Status;
        Term.Status = AcademicTermStatuses.Closed;
        _TermRepository.Update(Term);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Close",
            "AcademicTerms",
            nameof(AcademicTerm),
            Term.Id.ToString(CultureInfo.InvariantCulture),
            new { Status = PreviousStatus },
            new { Status = AcademicTermStatuses.Closed },
            Cancellation);

        return CatalogMapper.ToDto(Term);
    }

    private async Task EnsureUniqueCodeAsync(
        string Code,
        int? ExcludeTermId,
        CancellationToken Cancellation)
    {
        if (await _TermRepository.ExistsByCodeAsync(Code, ExcludeTermId, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "AcademicTerms.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }
    }

    private async Task EnsureNoOverlapAsync(
        DateOnly StartDate,
        DateOnly EndDate,
        int? ExcludeTermId,
        CancellationToken Cancellation)
    {
        var Overlapping = await _TermRepository.GetOverlappingAsync(
            StartDate,
            EndDate,
            ExcludeTermId,
            Cancellation);

        if (Overlapping is not null)
        {
            throw new BusinessRuleException(
                "AcademicTerms.Overlap",
                new Dictionary<string, string> { ["Code"] = Overlapping.Code });
        }
    }
}
