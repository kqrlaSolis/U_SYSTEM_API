// Reglas del material didáctico: CRUD con auditoría.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface ICourseMaterialService
{
    Task<PagedResult<CourseMaterialDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<CourseMaterialDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<CourseMaterialDto> CreateAsync(CreateCourseMaterialRequest Request, CancellationToken Cancellation = default);

    Task<CourseMaterialDto> UpdateAsync(int Id, UpdateCourseMaterialRequest Request, CancellationToken Cancellation = default);

    Task DeleteAsync(int Id, CancellationToken Cancellation = default);
}

public class CourseMaterialService : ICourseMaterialService
{
    private readonly ICourseMaterialRepository _MaterialRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public CourseMaterialService(
        ICourseMaterialRepository MaterialRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _MaterialRepository = MaterialRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<CourseMaterialDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _MaterialRepository.GetPagedAsync(Query, Cancellation);
        return PagedResult<CourseMaterialDto>.Create(
            Page.Items.Select(ToDto).ToList(),
            Page.TotalCount,
            Query);
    }

    public async Task<CourseMaterialDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Material = await _MaterialRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("CourseMaterials.NotFound");

        return ToDto(Material);
    }

    public async Task<CourseMaterialDto> CreateAsync(
        CreateCourseMaterialRequest Request,
        CancellationToken Cancellation = default)
    {
        var NewMaterial = new CourseMaterial
        {
            SubjectId = Request.SubjectId,
            AcademicTermId = Request.AcademicTermId,
            Type = Request.Type.Trim(),
            Title = Request.Title.Trim(),
            Description = Normalize(Request.Description),
            Url = Normalize(Request.Url),
            FilePath = Normalize(Request.FilePath),
            IsActive = true,
        };

        await _MaterialRepository.AddAsync(NewMaterial, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "CourseMaterials",
            nameof(CourseMaterial),
            NewMaterial.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewMaterial.SubjectId, NewMaterial.Type, NewMaterial.Title },
            Cancellation);

        return ToDto(NewMaterial);
    }

    public async Task<CourseMaterialDto> UpdateAsync(
        int Id,
        UpdateCourseMaterialRequest Request,
        CancellationToken Cancellation = default)
    {
        var Material = await _MaterialRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("CourseMaterials.NotFound");

        var Before = new { Material.SubjectId, Material.Type, Material.Title, Material.IsActive };

        Material.SubjectId = Request.SubjectId;
        Material.AcademicTermId = Request.AcademicTermId;
        Material.Type = Request.Type.Trim();
        Material.Title = Request.Title.Trim();
        Material.Description = Normalize(Request.Description);
        Material.Url = Normalize(Request.Url);
        Material.FilePath = Normalize(Request.FilePath);
        Material.IsActive = Request.IsActive;

        _MaterialRepository.Update(Material);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "CourseMaterials",
            nameof(CourseMaterial),
            Material.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Material.SubjectId, Material.Type, Material.Title, Material.IsActive },
            Cancellation);

        return ToDto(Material);
    }

    public async Task DeleteAsync(int Id, CancellationToken Cancellation = default)
    {
        var Material = await _MaterialRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("CourseMaterials.NotFound");

        _MaterialRepository.Remove(Material);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "CourseMaterials",
            nameof(CourseMaterial),
            Material.Id.ToString(CultureInfo.InvariantCulture),
            new { Material.SubjectId, Material.Title },
            null,
            Cancellation);
    }

    private static CourseMaterialDto ToDto(CourseMaterial Material)
        => new()
        {
            Id = Material.Id,
            SubjectId = Material.SubjectId,
            AcademicTermId = Material.AcademicTermId,
            Type = Material.Type,
            Title = Material.Title,
            Description = Material.Description,
            Url = Material.Url,
            FilePath = Material.FilePath,
            IsActive = Material.IsActive,
        };

    private static string? Normalize(string? Value)
        => string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();
}
