// Trámites: tipos con clave única y solicitudes con folio consecutivo.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IProcedureService
{
    Task<PagedResult<ProcedureTypeDto>> GetTypesAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<ProcedureTypeDto> GetTypeAsync(int Id, CancellationToken Cancellation = default);

    Task<ProcedureTypeDto> CreateTypeAsync(CreateProcedureTypeRequest Request, CancellationToken Cancellation = default);

    Task<ProcedureTypeDto> UpdateTypeAsync(int Id, UpdateProcedureTypeRequest Request, CancellationToken Cancellation = default);

    Task DeleteTypeAsync(int Id, CancellationToken Cancellation = default);

    Task<PagedResult<ProcedureDto>> GetProceduresAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<ProcedureDto> GetProcedureAsync(long Id, CancellationToken Cancellation = default);

    Task<ProcedureDto> CreateProcedureAsync(CreateProcedureRequest Request, CancellationToken Cancellation = default);

    Task<ProcedureDto> IssueAsync(long Id, IssueProcedureRequest Request, CancellationToken Cancellation = default);
}

public class ProcedureService : IProcedureService
{
    private readonly IProcedureRepository _ProcedureRepository;
    private readonly ICurrentUser _CurrentUser;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public ProcedureService(
        IProcedureRepository ProcedureRepository,
        ICurrentUser CurrentUser,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _ProcedureRepository = ProcedureRepository;
        _CurrentUser = CurrentUser;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<ProcedureTypeDto>> GetTypesAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _ProcedureRepository.GetTypesPagedAsync(Query, Cancellation);
        return PagedResult<ProcedureTypeDto>.Create(
            Page.Items.Select(ToTypeDto).ToList(),
            Page.TotalCount,
            Query);
    }

    public async Task<ProcedureTypeDto> GetTypeAsync(int Id, CancellationToken Cancellation = default)
    {
        var Type = await _ProcedureRepository.GetTypeByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Procedures.TypeNotFound");

        return ToTypeDto(Type);
    }

    public async Task<ProcedureTypeDto> CreateTypeAsync(
        CreateProcedureTypeRequest Request,
        CancellationToken Cancellation = default)
    {
        var Code = Request.Code.Trim().ToUpperInvariant();

        if (await _ProcedureRepository.ExistsTypeByCodeAsync(Code, null, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Procedures.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var NewType = new ProcedureType
        {
            Code = Code,
            Name = Request.Name.Trim(),
            PaymentConceptId = Request.PaymentConceptId,
            IsActive = true,
        };

        await _ProcedureRepository.AddTypeAsync(NewType, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Procedures",
            nameof(ProcedureType),
            NewType.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewType.Code, NewType.Name },
            Cancellation);

        return ToTypeDto(NewType);
    }

    public async Task<ProcedureTypeDto> UpdateTypeAsync(
        int Id,
        UpdateProcedureTypeRequest Request,
        CancellationToken Cancellation = default)
    {
        var Type = await _ProcedureRepository.GetTypeByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Procedures.TypeNotFound");

        var Code = Request.Code.Trim().ToUpperInvariant();
        if (await _ProcedureRepository.ExistsTypeByCodeAsync(Code, Type.Id, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Procedures.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var Before = new { Type.Code, Type.Name, Type.IsActive };

        Type.Code = Code;
        Type.Name = Request.Name.Trim();
        Type.PaymentConceptId = Request.PaymentConceptId;
        Type.IsActive = Request.IsActive;

        _ProcedureRepository.UpdateType(Type);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Procedures",
            nameof(ProcedureType),
            Type.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Type.Code, Type.Name, Type.IsActive },
            Cancellation);

        return ToTypeDto(Type);
    }

    public async Task DeleteTypeAsync(int Id, CancellationToken Cancellation = default)
    {
        var Type = await _ProcedureRepository.GetTypeByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Procedures.TypeNotFound");

        _ProcedureRepository.RemoveType(Type);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Procedures",
            nameof(ProcedureType),
            Type.Id.ToString(CultureInfo.InvariantCulture),
            new { Type.Code, Type.Name },
            null,
            Cancellation);
    }

    public async Task<PagedResult<ProcedureDto>> GetProceduresAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _ProcedureRepository.GetPagedAsync(Query, Cancellation);
        var Items = new List<ProcedureDto>();

        foreach (var Procedure in Page.Items)
        {
            var Type = await _ProcedureRepository.GetTypeByIdAsync(Procedure.ProcedureTypeId, Cancellation);
            Items.Add(ToDto(Procedure, Type?.Code ?? string.Empty));
        }

        return PagedResult<ProcedureDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<ProcedureDto> GetProcedureAsync(long Id, CancellationToken Cancellation = default)
    {
        var Procedure = await _ProcedureRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Procedures.NotFound");

        var Type = await _ProcedureRepository.GetTypeByIdAsync(Procedure.ProcedureTypeId, Cancellation);
        return ToDto(Procedure, Type?.Code ?? string.Empty);
    }

    public async Task<ProcedureDto> CreateProcedureAsync(
        CreateProcedureRequest Request,
        CancellationToken Cancellation = default)
    {
        var Type = await _ProcedureRepository.GetTypeByIdAsync(Request.ProcedureTypeId, Cancellation)
            ?? throw new NotFoundException("Procedures.TypeNotFound");

        if (!Type.IsActive)
        {
            throw BusinessRuleException.Conflict("Procedures.TypeInactive");
        }

        var NewProcedure = new Procedure
        {
            ProcedureTypeId = Type.Id,
            StudentId = Request.StudentId,
            FolioNumber = await BuildFolioAsync(Cancellation),
            Status = "Requested",
            RequestedAt = DateTime.UtcNow,
            // El usuario que atiende sale del token: la FK no admite 0
            UserId = Request.UserId ?? _CurrentUser.UserId ?? throw new BusinessRuleException("Procedures.UserRequired"),
        };

        await _ProcedureRepository.AddAsync(NewProcedure, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Request",
            "Procedures",
            nameof(Procedure),
            NewProcedure.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewProcedure.FolioNumber, Type.Code, NewProcedure.StudentId },
            Cancellation);

        return ToDto(NewProcedure, Type.Code);
    }

    public async Task<ProcedureDto> IssueAsync(
        long Id,
        IssueProcedureRequest Request,
        CancellationToken Cancellation = default)
    {
        var Procedure = await _ProcedureRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Procedures.NotFound");

        if (string.Equals(Procedure.Status, "Issued", StringComparison.OrdinalIgnoreCase))
        {
            throw BusinessRuleException.Conflict("Procedures.AlreadyIssued");
        }

        var Before = new { Procedure.Status, Procedure.IssuedAt };

        Procedure.Status = "Issued";
        Procedure.IssuedAt = DateTime.UtcNow;
        Procedure.FilePath = string.IsNullOrWhiteSpace(Request.FilePath) ? Procedure.FilePath : Request.FilePath.Trim();

        _ProcedureRepository.Update(Procedure);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Issue",
            "Procedures",
            nameof(Procedure),
            Procedure.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Procedure.Status, Procedure.IssuedAt },
            Cancellation);

        var Type = await _ProcedureRepository.GetTypeByIdAsync(Procedure.ProcedureTypeId, Cancellation);
        return ToDto(Procedure, Type?.Code ?? string.Empty);
    }

    // Folio: TRM + año + consecutivo de 4 dígitos (TRM20260001)
    private async Task<string> BuildFolioAsync(CancellationToken Cancellation)
    {
        var Prefix = $"TRM{DateTime.UtcNow.Year}";
        var Last = await _ProcedureRepository.GetLastFolioAsync(Prefix, Cancellation);

        var Sequence = 1;
        if (Last is not null && Last.Length > Prefix.Length
            && int.TryParse(Last[Prefix.Length..], NumberStyles.Integer, CultureInfo.InvariantCulture, out var Parsed))
        {
            Sequence = Parsed + 1;
        }

        return $"{Prefix}{Sequence.ToString("D4", CultureInfo.InvariantCulture)}";
    }

    private static ProcedureTypeDto ToTypeDto(ProcedureType Type)
        => new()
        {
            Id = Type.Id,
            Code = Type.Code,
            Name = Type.Name,
            PaymentConceptId = Type.PaymentConceptId,
            IsActive = Type.IsActive,
        };

    private static ProcedureDto ToDto(Procedure Procedure, string TypeCode)
        => new()
        {
            Id = Procedure.Id,
            ProcedureTypeId = Procedure.ProcedureTypeId,
            ProcedureTypeCode = TypeCode,
            StudentId = Procedure.StudentId,
            FolioNumber = Procedure.FolioNumber,
            Status = Procedure.Status,
            RequestedAt = Procedure.RequestedAt,
            IssuedAt = Procedure.IssuedAt,
            FilePath = Procedure.FilePath,
            UserId = Procedure.UserId,
        };
}
