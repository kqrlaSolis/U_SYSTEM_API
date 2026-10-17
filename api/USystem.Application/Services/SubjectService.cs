// Reglas de materias: clave única y baja bloqueada cuando la materia está en uso.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _SubjectRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public SubjectService(
        ISubjectRepository SubjectRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _SubjectRepository = SubjectRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<SubjectDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _SubjectRepository.GetPagedAsync(Query, Cancellation);
        var Items = Page.Items.Select(CatalogMapper.ToDto).ToList();
        return PagedResult<SubjectDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<SubjectDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Subject = await _SubjectRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Subjects.NotFound");

        return CatalogMapper.ToDto(Subject);
    }

    public async Task<SubjectDto> CreateAsync(
        CreateSubjectRequest Request,
        CancellationToken Cancellation = default)
    {
        var Code = Request.Code.Trim();

        if (await _SubjectRepository.ExistsByCodeAsync(Code, null, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Subjects.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var NewSubject = new Subject
        {
            Code = Code,
            Name = Request.Name.Trim(),
            Credits = Request.Credits,
            TheoryHours = Request.TheoryHours,
            PracticeHours = Request.PracticeHours,
            IsActive = true,
        };

        await _SubjectRepository.AddAsync(NewSubject, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Subjects",
            nameof(Subject),
            NewSubject.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewSubject.Code, NewSubject.Name, NewSubject.Credits },
            Cancellation);

        return CatalogMapper.ToDto(NewSubject);
    }

    public async Task<SubjectDto> UpdateAsync(
        int Id,
        UpdateSubjectRequest Request,
        CancellationToken Cancellation = default)
    {
        var Subject = await _SubjectRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Subjects.NotFound");

        var Code = Request.Code.Trim();
        if (await _SubjectRepository.ExistsByCodeAsync(Code, Subject.Id, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Subjects.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var Before = new { Subject.Code, Subject.Name, Subject.Credits, Subject.TheoryHours, Subject.PracticeHours, Subject.IsActive };

        Subject.Code = Code;
        Subject.Name = Request.Name.Trim();
        Subject.Credits = Request.Credits;
        Subject.TheoryHours = Request.TheoryHours;
        Subject.PracticeHours = Request.PracticeHours;
        Subject.IsActive = Request.IsActive;

        _SubjectRepository.Update(Subject);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Subjects",
            nameof(Subject),
            Subject.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Subject.Code, Subject.Name, Subject.Credits, Subject.TheoryHours, Subject.PracticeHours, Subject.IsActive },
            Cancellation);

        return CatalogMapper.ToDto(Subject);
    }

    public async Task DeleteAsync(int Id, CancellationToken Cancellation = default)
    {
        var Subject = await _SubjectRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Subjects.NotFound");

        var UsedInPlans = await _SubjectRepository.IsUsedInStudyPlansAsync(Subject.Id, Cancellation);
        var UsedInGroups = await _SubjectRepository.IsUsedInClassGroupsAsync(Subject.Id, Cancellation);

        if (UsedInPlans || UsedInGroups)
        {
            throw new BusinessRuleException(
                "Subjects.InUse",
                new Dictionary<string, string> { ["Code"] = Subject.Code });
        }

        _SubjectRepository.Remove(Subject);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Subjects",
            nameof(Subject),
            Subject.Id.ToString(CultureInfo.InvariantCulture),
            new { Subject.Code, Subject.Name },
            null,
            Cancellation);
    }
}
