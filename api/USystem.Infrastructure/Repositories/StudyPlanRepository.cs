// Acceso a datos de los planes de estudio, su malla y su seriación.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class StudyPlanRepository : IStudyPlanRepository
{
    private readonly USystemDbContext _Context;

    public StudyPlanRepository(USystemDbContext Context) => _Context = Context;

    public Task<StudyPlan?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.StudyPlans.FirstOrDefaultAsync(Plan => Plan.Id == Id, Cancellation);

    public Task<bool> ExistsByCodeVersionAsync(
        int CareerId,
        string Code,
        string Version,
        int? ExcludePlanId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        // Con IncludeDeleted=true también cuenta el plan dado de baja: el índice único no admite repetidos
        var Source = IncludeDeleted ? _Context.StudyPlans.IgnoreQueryFilters() : _Context.StudyPlans;
        return Source.AnyAsync(
            Plan => Plan.CareerId == CareerId
                 && Plan.Code == Code
                 && Plan.Version == Version
                 && (ExcludePlanId == null || Plan.Id != ExcludePlanId),
            Cancellation);
    }

    public Task<bool> HasStudentsAsync(int StudyPlanId, CancellationToken Cancellation = default)
        => _Context.Students.AnyAsync(Student => Student.StudyPlanId == StudyPlanId, Cancellation);

    public Task<StudyPlan?> GetCurrentByCareerAsync(int CareerId, CancellationToken Cancellation = default)
        => _Context.StudyPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(Plan => Plan.CareerId == CareerId && Plan.IsCurrent, Cancellation);

    public async Task ClearCurrentAsync(
        int CareerId,
        int? KeepPlanId = null,
        CancellationToken Cancellation = default)
    {
        var Plans = await _Context.StudyPlans
            .Where(Plan => Plan.CareerId == CareerId && Plan.IsCurrent && Plan.Id != KeepPlanId)
            .ToListAsync(Cancellation);

        foreach (var Plan in Plans)
        {
            Plan.IsCurrent = false;
        }
    }

    public async Task<IReadOnlyList<StudyPlanSubject>> GetSubjectsAsync(
        int StudyPlanId,
        CancellationToken Cancellation = default)
        => await _Context.StudyPlanSubjects
            .AsNoTracking()
            .Where(PlanSubject => PlanSubject.StudyPlanId == StudyPlanId)
            .ToListAsync(Cancellation);

    public async Task<IReadOnlyList<SubjectPrerequisite>> GetPrerequisitesAsync(
        int StudyPlanId,
        CancellationToken Cancellation = default)
        => await _Context.SubjectPrerequisites
            .AsNoTracking()
            .Where(Prerequisite => Prerequisite.StudyPlanId == StudyPlanId)
            .ToListAsync(Cancellation);

    // La malla se reemplaza por diferencia para no chocar con la llave compuesta
    public async Task ReplaceSubjectsAsync(
        int StudyPlanId,
        IReadOnlyCollection<StudyPlanSubject> Subjects,
        CancellationToken Cancellation = default)
    {
        var Desired = Subjects.ToDictionary(PlanSubject => PlanSubject.SubjectId);
        var Existing = await _Context.StudyPlanSubjects
            .Where(PlanSubject => PlanSubject.StudyPlanId == StudyPlanId)
            .ToListAsync(Cancellation);

        var ToRemove = Existing.Where(PlanSubject => !Desired.ContainsKey(PlanSubject.SubjectId)).ToList();
        _Context.StudyPlanSubjects.RemoveRange(ToRemove);

        foreach (var PlanSubject in Existing)
        {
            if (!Desired.TryGetValue(PlanSubject.SubjectId, out var Wanted))
            {
                continue;
            }

            PlanSubject.TermNumber = Wanted.TermNumber;
            PlanSubject.Type = Wanted.Type;
        }

        var ExistingIds = Existing.Select(PlanSubject => PlanSubject.SubjectId).ToHashSet();
        foreach (var PlanSubject in Desired.Values.Where(Item => !ExistingIds.Contains(Item.SubjectId)))
        {
            await _Context.StudyPlanSubjects.AddAsync(PlanSubject, Cancellation);
        }
    }

    public async Task ReplacePrerequisitesAsync(
        int StudyPlanId,
        IReadOnlyCollection<SubjectPrerequisite> Prerequisites,
        CancellationToken Cancellation = default)
    {
        var Desired = Prerequisites
            .Select(Prerequisite => (Prerequisite.SubjectId, Prerequisite.PrerequisiteSubjectId))
            .ToHashSet();

        var Existing = await _Context.SubjectPrerequisites
            .Where(Prerequisite => Prerequisite.StudyPlanId == StudyPlanId)
            .ToListAsync(Cancellation);

        var ToRemove = Existing
            .Where(Prerequisite => !Desired.Contains((Prerequisite.SubjectId, Prerequisite.PrerequisiteSubjectId)))
            .ToList();
        _Context.SubjectPrerequisites.RemoveRange(ToRemove);

        var ExistingEdges = Existing
            .Select(Prerequisite => (Prerequisite.SubjectId, Prerequisite.PrerequisiteSubjectId))
            .ToHashSet();

        foreach (var Prerequisite in Prerequisites
                     .Where(Item => !ExistingEdges.Contains((Item.SubjectId, Item.PrerequisiteSubjectId))))
        {
            await _Context.SubjectPrerequisites.AddAsync(Prerequisite, Cancellation);
        }
    }

    public async Task<PagedResult<StudyPlan>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.StudyPlans.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Plan => Plan.Code.Contains(Search) || Plan.Version.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderByDescending(Plan => Plan.StartDate)
            .ThenBy(Plan => Plan.Code)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<StudyPlan>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(StudyPlan Entity, CancellationToken Cancellation = default)
        => await _Context.StudyPlans.AddAsync(Entity, Cancellation);

    public void Update(StudyPlan Entity) => _Context.StudyPlans.Update(Entity);

    public void Remove(StudyPlan Entity) => _Context.StudyPlans.Remove(Entity);
}
