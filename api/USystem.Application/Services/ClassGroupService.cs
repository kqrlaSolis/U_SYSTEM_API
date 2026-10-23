// Reglas de grupos: código único por periodo, horarios válidos y baja bloqueada con alumnos.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IClassGroupService
{
    Task<PagedResult<ClassGroupDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<ClassGroupDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<ClassGroupDto> CreateAsync(CreateClassGroupRequest Request, CancellationToken Cancellation = default);

    Task<ClassGroupDto> UpdateAsync(int Id, UpdateClassGroupRequest Request, CancellationToken Cancellation = default);

    Task DeleteAsync(int Id, CancellationToken Cancellation = default);
}

public class ClassGroupService : IClassGroupService
{
    private readonly IClassGroupRepository _GroupRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public ClassGroupService(
        IClassGroupRepository GroupRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _GroupRepository = GroupRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<ClassGroupDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _GroupRepository.GetPagedAsync(Query, Cancellation);
        var Items = new List<ClassGroupDto>();

        foreach (var Group in Page.Items)
        {
            var Schedules = await _GroupRepository.GetSchedulesAsync(Group.Id, Cancellation);
            Items.Add(ToDto(Group, Schedules));
        }

        return PagedResult<ClassGroupDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<ClassGroupDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Group = await _GroupRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("ClassGroups.NotFound");

        return ToDto(Group, await _GroupRepository.GetSchedulesAsync(Id, Cancellation));
    }

    public async Task<ClassGroupDto> CreateAsync(
        CreateClassGroupRequest Request,
        CancellationToken Cancellation = default)
    {
        await EnsureCodeAvailableAsync(Request.AcademicTermId, Request.Code, null, Cancellation);

        var NewGroup = new ClassGroup
        {
            AcademicTermId = Request.AcademicTermId,
            SubjectId = Request.SubjectId,
            TeacherId = Request.TeacherId,
            Code = Request.Code.Trim(),
            Classroom = Normalize(Request.Classroom),
            Capacity = Request.Capacity,
            EnrolledCount = 0,
            Status = Request.Status.Trim(),
        };

        await _GroupRepository.AddAsync(NewGroup, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        foreach (var Schedule in Request.Schedules)
        {
            await _GroupRepository.AddScheduleAsync(
                new ClassGroupSchedule
                {
                    ClassGroupId = NewGroup.Id,
                    DayOfWeek = (byte)Schedule.DayOfWeek,
                    StartTime = ClassGroupScheduleRequest.ParseTime(Schedule.StartTime),
                    EndTime = ClassGroupScheduleRequest.ParseTime(Schedule.EndTime),
                },
                Cancellation);
        }

        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "ClassGroups",
            nameof(ClassGroup),
            NewGroup.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewGroup.AcademicTermId, NewGroup.SubjectId, NewGroup.TeacherId, NewGroup.Code },
            Cancellation);

        return ToDto(NewGroup, await _GroupRepository.GetSchedulesAsync(NewGroup.Id, Cancellation));
    }

    public async Task<ClassGroupDto> UpdateAsync(
        int Id,
        UpdateClassGroupRequest Request,
        CancellationToken Cancellation = default)
    {
        var Group = await _GroupRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("ClassGroups.NotFound");

        await EnsureCodeAvailableAsync(Request.AcademicTermId, Request.Code, Group.Id, Cancellation);

        var Before = new { Group.AcademicTermId, Group.SubjectId, Group.TeacherId, Group.Code };

        Group.AcademicTermId = Request.AcademicTermId;
        Group.SubjectId = Request.SubjectId;
        Group.TeacherId = Request.TeacherId;
        Group.Code = Request.Code.Trim();
        Group.Classroom = Normalize(Request.Classroom);
        Group.Capacity = Request.Capacity;
        Group.Status = Request.Status.Trim();

        _GroupRepository.Update(Group);

        var ExistingSchedules = await _GroupRepository.GetSchedulesAsync(Group.Id, Cancellation);
        if (ExistingSchedules.Count > 0)
        {
            _GroupRepository.RemoveSchedules(ExistingSchedules);
        }

        foreach (var Schedule in Request.Schedules)
        {
            await _GroupRepository.AddScheduleAsync(
                new ClassGroupSchedule
                {
                    ClassGroupId = Group.Id,
                    DayOfWeek = (byte)Schedule.DayOfWeek,
                    StartTime = ClassGroupScheduleRequest.ParseTime(Schedule.StartTime),
                    EndTime = ClassGroupScheduleRequest.ParseTime(Schedule.EndTime),
                },
                Cancellation);
        }

        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "ClassGroups",
            nameof(ClassGroup),
            Group.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Group.AcademicTermId, Group.SubjectId, Group.TeacherId, Group.Code },
            Cancellation);

        return ToDto(Group, await _GroupRepository.GetSchedulesAsync(Group.Id, Cancellation));
    }

    public async Task DeleteAsync(int Id, CancellationToken Cancellation = default)
    {
        var Group = await _GroupRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("ClassGroups.NotFound");

        if (await _GroupRepository.HasEnrollmentsAsync(Group.Id, Cancellation))
        {
            throw new BusinessRuleException(
                "ClassGroups.InUse",
                new Dictionary<string, string> { ["Code"] = Group.Code });
        }

        _GroupRepository.Remove(Group);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "ClassGroups",
            nameof(ClassGroup),
            Group.Id.ToString(CultureInfo.InvariantCulture),
            new { Group.Code, Group.SubjectId, Group.TeacherId },
            null,
            Cancellation);
    }

    private async Task EnsureCodeAvailableAsync(
        int AcademicTermId,
        string Code,
        int? ExcludeClassGroupId,
        CancellationToken Cancellation)
    {
        var Trimmed = Code.Trim();
        if (await _GroupRepository.ExistsByCodeAsync(AcademicTermId, Trimmed, ExcludeClassGroupId, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "ClassGroups.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Trimmed });
        }
    }

    private static ClassGroupDto ToDto(
        ClassGroup Group,
        IReadOnlyList<ClassGroupSchedule> Schedules)
        => new()
        {
            Id = Group.Id,
            AcademicTermId = Group.AcademicTermId,
            SubjectId = Group.SubjectId,
            TeacherId = Group.TeacherId,
            Code = Group.Code,
            Classroom = Group.Classroom,
            Capacity = Group.Capacity,
            EnrolledCount = Group.EnrolledCount,
            Status = Group.Status,
            Schedules = Schedules
                .Select(Schedule => new ClassGroupScheduleDto
                {
                    DayOfWeek = Schedule.DayOfWeek,
                    StartTime = Schedule.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                    EndTime = Schedule.EndTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                })
                .ToList(),
        };

    private static string? Normalize(string? Value)
        => string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();
}
