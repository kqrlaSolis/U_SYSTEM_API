// Reglas de carreras: clave única y baja bloqueada si tiene planes o alumnos.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class CareerService : ICareerService
{
    private readonly ICareerRepository _CareerRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public CareerService(
        ICareerRepository CareerRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _CareerRepository = CareerRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<CareerDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _CareerRepository.GetPagedAsync(Query, Cancellation);
        var Items = Page.Items.Select(CatalogMapper.ToDto).ToList();
        return PagedResult<CareerDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<CareerDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Career = await _CareerRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Careers.NotFound");

        return CatalogMapper.ToDto(Career);
    }

    public async Task<CareerDto> CreateAsync(
        CreateCareerRequest Request,
        CancellationToken Cancellation = default)
    {
        var Code = Request.Code.Trim();

        if (await _CareerRepository.ExistsByCodeAsync(Code, null, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Careers.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var NewCareer = new Career
        {
            Code = Code,
            Name = Request.Name.Trim(),
            Level = Request.Level.Trim(),
            TermCount = Request.TermCount,
            TotalCredits = Request.TotalCredits,
            Modality = Request.Modality.Trim(),
            IsActive = true,
        };

        await _CareerRepository.AddAsync(NewCareer, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Careers",
            nameof(Career),
            NewCareer.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewCareer.Code, NewCareer.Name },
            Cancellation);

        return CatalogMapper.ToDto(NewCareer);
    }

    public async Task<CareerDto> UpdateAsync(
        int Id,
        UpdateCareerRequest Request,
        CancellationToken Cancellation = default)
    {
        var Career = await _CareerRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Careers.NotFound");

        var Code = Request.Code.Trim();
        if (await _CareerRepository.ExistsByCodeAsync(Code, Career.Id, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Careers.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var Before = new { Career.Code, Career.Name, Career.Level, Career.TermCount, Career.TotalCredits, Career.Modality, Career.IsActive };

        Career.Code = Code;
        Career.Name = Request.Name.Trim();
        Career.Level = Request.Level.Trim();
        Career.TermCount = Request.TermCount;
        Career.TotalCredits = Request.TotalCredits;
        Career.Modality = Request.Modality.Trim();
        Career.IsActive = Request.IsActive;

        _CareerRepository.Update(Career);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Careers",
            nameof(Career),
            Career.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Career.Code, Career.Name, Career.Level, Career.TermCount, Career.TotalCredits, Career.Modality, Career.IsActive },
            Cancellation);

        return CatalogMapper.ToDto(Career);
    }

    public async Task DeleteAsync(int Id, CancellationToken Cancellation = default)
    {
        var Career = await _CareerRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Careers.NotFound");

        var HasPlans = await _CareerRepository.HasStudyPlansAsync(Career.Id, Cancellation);
        var HasStudents = await _CareerRepository.HasStudentsAsync(Career.Id, Cancellation);

        // Con historial solo se desactiva: nunca se borra
        if (HasPlans || HasStudents)
        {
            throw new BusinessRuleException(
                "Careers.InUse",
                new Dictionary<string, string> { ["Code"] = Career.Code });
        }

        _CareerRepository.Remove(Career);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Careers",
            nameof(Career),
            Career.Id.ToString(CultureInfo.InvariantCulture),
            new { Career.Code, Career.Name },
            null,
            Cancellation);
    }
}
