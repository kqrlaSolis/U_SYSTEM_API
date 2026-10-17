// Reglas de los planes de estudio: versiones, créditos, seriación sin ciclos y plan vigente.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class StudyPlanService : IStudyPlanService
{
    private readonly IStudyPlanRepository _StudyPlanRepository;
    private readonly ICareerRepository _CareerRepository;
    private readonly ISubjectRepository _SubjectRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public StudyPlanService(
        IStudyPlanRepository StudyPlanRepository,
        ICareerRepository CareerRepository,
        ISubjectRepository SubjectRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _StudyPlanRepository = StudyPlanRepository;
        _CareerRepository = CareerRepository;
        _SubjectRepository = SubjectRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<StudyPlanDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _StudyPlanRepository.GetPagedAsync(Query, Cancellation);

        var CareerIds = Page.Items.Select(Plan => Plan.CareerId).Distinct().ToArray();
        var Careers = await _CareerRepository.GetByIdsAsync(CareerIds, Cancellation);
        var CareerNames = Careers.ToDictionary(Career => Career.Id, Career => Career.Name);

        var Items = Page.Items
            .Select(Plan => StudyPlanMapper.ToDto(
                Plan,
                CareerNames.TryGetValue(Plan.CareerId, out var CareerName) ? CareerName : string.Empty,
                new Dictionary<int, Subject>(),
                Array.Empty<StudyPlanSubject>(),
                Array.Empty<SubjectPrerequisite>()))
            .ToList();

        return PagedResult<StudyPlanDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<StudyPlanDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Plan = await _StudyPlanRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("StudyPlans.NotFound");

        return await BuildDtoAsync(Plan, Cancellation);
    }

    public async Task<StudyPlanDto> CreateAsync(
        CreateStudyPlanRequest Request,
        CancellationToken Cancellation = default)
    {
        var Career = await _CareerRepository.GetByIdAsync(Request.CareerId, Cancellation)
            ?? throw new NotFoundException("Careers.NotFound");

        var Code = Request.Code.Trim();
        var Version = Request.Version.Trim();
        await EnsureUniqueAsync(Career.Id, Code, Version, null, Cancellation);

        var NewPlan = new StudyPlan
        {
            CareerId = Career.Id,
            Code = Code,
            Version = Version,
            StartDate = Request.StartDate,
            EndDate = Request.EndDate,
            TotalCredits = Request.TotalCredits,
            IsCurrent = false,
        };

        await _StudyPlanRepository.AddAsync(NewPlan, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "StudyPlans",
            nameof(StudyPlan),
            NewPlan.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewPlan.Code, NewPlan.Version, NewPlan.CareerId },
            Cancellation);

        return await GetByIdAsync(NewPlan.Id, Cancellation);
    }

    public async Task<StudyPlanDto> UpdateAsync(
        int Id,
        UpdateStudyPlanRequest Request,
        CancellationToken Cancellation = default)
    {
        var Plan = await _StudyPlanRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("StudyPlans.NotFound");

        var Career = await _CareerRepository.GetByIdAsync(Request.CareerId, Cancellation)
            ?? throw new NotFoundException("Careers.NotFound");

        var Code = Request.Code.Trim();
        var Version = Request.Version.Trim();
        await EnsureUniqueAsync(Career.Id, Code, Version, Plan.Id, Cancellation);

        var Before = new { Plan.CareerId, Plan.Code, Plan.Version, Plan.StartDate, Plan.EndDate, Plan.TotalCredits };

        Plan.CareerId = Career.Id;
        Plan.Code = Code;
        Plan.Version = Version;
        Plan.StartDate = Request.StartDate;
        Plan.EndDate = Request.EndDate;
        Plan.TotalCredits = Request.TotalCredits;

        _StudyPlanRepository.Update(Plan);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "StudyPlans",
            nameof(StudyPlan),
            Plan.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Plan.CareerId, Plan.Code, Plan.Version, Plan.StartDate, Plan.EndDate, Plan.TotalCredits },
            Cancellation);

        return await BuildDtoAsync(Plan, Cancellation);
    }

    public async Task DeleteAsync(int Id, CancellationToken Cancellation = default)
    {
        var Plan = await _StudyPlanRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("StudyPlans.NotFound");

        // Un plan en uso conserva su estructura: se crea una nueva versión
        if (await _StudyPlanRepository.HasStudentsAsync(Plan.Id, Cancellation))
        {
            throw new BusinessRuleException("StudyPlans.InUse");
        }

        if (Plan.IsCurrent)
        {
            throw BusinessRuleException.Conflict(
                "StudyPlans.CurrentCannotDelete",
                new Dictionary<string, string> { ["Code"] = Plan.Code });
        }

        _StudyPlanRepository.Remove(Plan);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "StudyPlans",
            nameof(StudyPlan),
            Plan.Id.ToString(CultureInfo.InvariantCulture),
            new { Plan.Code, Plan.Version },
            null,
            Cancellation);
    }

    public async Task<StudyPlanDto> ReplaceSubjectsAsync(
        int Id,
        StudyPlanSubjectsRequest Request,
        CancellationToken Cancellation = default)
    {
        var Plan = await _StudyPlanRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("StudyPlans.NotFound");

        await EnsureStructureCanChangeAsync(Plan, Cancellation);

        var Career = await _CareerRepository.GetByIdAsync(Plan.CareerId, Cancellation)
            ?? throw new NotFoundException("Careers.NotFound");

        var Items = Request.Subjects
            .GroupBy(Item => Item.SubjectId)
            .Select(Group => Group.First())
            .ToList();

        if (Items.Count != Request.Subjects.Count)
        {
            throw new ValidationException("Validation.InvalidFormat", nameof(Request.Subjects));
        }

        var Subjects = await _SubjectRepository.GetByIdsAsync(
            Items.Select(Item => Item.SubjectId).ToArray(),
            Cancellation);
        var SubjectsById = Subjects.ToDictionary(Subject => Subject.Id);

        if (SubjectsById.Count != Items.Count)
        {
            throw new ValidationException("Validation.InvalidFormat", nameof(Request.Subjects));
        }

        if (Items.Any(Item => Item.TermNumber > Career.TermCount))
        {
            throw new ValidationException("Validation.OutOfRange", nameof(Request.Subjects));
        }

        var AssignedCredits = Items.Sum(Item => (int)SubjectsById[Item.SubjectId].Credits);
        if (AssignedCredits != Plan.TotalCredits)
        {
            throw new BusinessRuleException(
                "StudyPlans.CreditsMismatch",
                new Dictionary<string, string>
                {
                    ["SubjectCredits"] = AssignedCredits.ToString(CultureInfo.InvariantCulture),
                    ["PlanCredits"] = Plan.TotalCredits.ToString(CultureInfo.InvariantCulture),
                });
        }

        var PlanSubjects = Items
            .Select(Item => new StudyPlanSubject
            {
                StudyPlanId = Plan.Id,
                SubjectId = Item.SubjectId,
                TermNumber = Item.TermNumber,
                Type = Item.Type,
            })
            .ToList();

        await _StudyPlanRepository.ReplaceSubjectsAsync(Plan.Id, PlanSubjects, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "StudyPlans",
            nameof(StudyPlan),
            Plan.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { Subjects = PlanSubjects.Count, Credits = AssignedCredits },
            Cancellation);

        return await BuildDtoAsync(Plan, Cancellation);
    }

    public async Task<StudyPlanDto> ReplacePrerequisitesAsync(
        int Id,
        StudyPlanPrerequisitesRequest Request,
        CancellationToken Cancellation = default)
    {
        var Plan = await _StudyPlanRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("StudyPlans.NotFound");

        await EnsureStructureCanChangeAsync(Plan, Cancellation);

        var CurrentSubjects = await _StudyPlanRepository.GetSubjectsAsync(Plan.Id, Cancellation);
        var AllowedSubjectIds = CurrentSubjects.Select(PlanSubject => PlanSubject.SubjectId).ToHashSet();

        var Edges = Request.Prerequisites
            .Where(Item => AllowedSubjectIds.Contains(Item.SubjectId)
                        && AllowedSubjectIds.Contains(Item.PrerequisiteSubjectId))
            .GroupBy(Item => (Item.SubjectId, Item.PrerequisiteSubjectId))
            .Select(Group => Group.First())
            .ToList();

        if (Edges.Count != Request.Prerequisites.Count)
        {
            throw new ValidationException("Validation.InvalidFormat", nameof(Request.Prerequisites));
        }

        var Prerequisites = Edges
            .Select(Edge => new SubjectPrerequisite
            {
                StudyPlanId = Plan.Id,
                SubjectId = Edge.SubjectId,
                PrerequisiteSubjectId = Edge.PrerequisiteSubjectId,
            })
            .ToList();

        if (HasCycle(Prerequisites))
        {
            throw new BusinessRuleException("StudyPlans.CircularPrerequisite");
        }

        await _StudyPlanRepository.ReplacePrerequisitesAsync(Plan.Id, Prerequisites, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "StudyPlans",
            nameof(StudyPlan),
            Plan.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { Prerequisites = Prerequisites.Count },
            Cancellation);

        return await BuildDtoAsync(Plan, Cancellation);
    }

    public async Task<StudyPlanDto> SetCurrentAsync(int Id, CancellationToken Cancellation = default)
    {
        var Plan = await _StudyPlanRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("StudyPlans.NotFound");

        var PlanSubjects = await _StudyPlanRepository.GetSubjectsAsync(Plan.Id, Cancellation);
        var Subjects = await _SubjectRepository.GetByIdsAsync(
            PlanSubjects.Select(PlanSubject => PlanSubject.SubjectId).ToArray(),
            Cancellation);
        var SubjectsById = Subjects.ToDictionary(Subject => Subject.Id);
        var AssignedCredits = StudyPlanMapper.SumCredits(PlanSubjects, SubjectsById);

        // Publicar exige que la malla cubra los créditos del plan
        if (AssignedCredits != Plan.TotalCredits)
        {
            throw new BusinessRuleException(
                "StudyPlans.CreditsMismatch",
                new Dictionary<string, string>
                {
                    ["SubjectCredits"] = AssignedCredits.ToString(CultureInfo.InvariantCulture),
                    ["PlanCredits"] = Plan.TotalCredits.ToString(CultureInfo.InvariantCulture),
                });
        }

        // Solo un plan vigente por carrera: el anterior deja de serlo
        await _StudyPlanRepository.ClearCurrentAsync(Plan.CareerId, Plan.Id, Cancellation);
        Plan.IsCurrent = true;
        _StudyPlanRepository.Update(Plan);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "SetCurrent",
            "StudyPlans",
            nameof(StudyPlan),
            Plan.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { Plan.Code, Plan.Version, Plan.IsCurrent },
            Cancellation);

        return await BuildDtoAsync(Plan, Cancellation);
    }

    private async Task<StudyPlanDto> BuildDtoAsync(StudyPlan Plan, CancellationToken Cancellation)
    {
        var Career = await _CareerRepository.GetByIdAsync(Plan.CareerId, Cancellation);
        var PlanSubjects = await _StudyPlanRepository.GetSubjectsAsync(Plan.Id, Cancellation);
        var Prerequisites = await _StudyPlanRepository.GetPrerequisitesAsync(Plan.Id, Cancellation);

        var SubjectIds = PlanSubjects
            .Select(PlanSubject => PlanSubject.SubjectId)
            .Concat(Prerequisites.Select(Prerequisite => Prerequisite.PrerequisiteSubjectId))
            .Distinct()
            .ToArray();

        var Subjects = await _SubjectRepository.GetByIdsAsync(SubjectIds, Cancellation);
        var SubjectsById = Subjects.ToDictionary(Subject => Subject.Id);

        return StudyPlanMapper.ToDto(
            Plan,
            Career?.Name ?? string.Empty,
            SubjectsById,
            PlanSubjects,
            Prerequisites);
    }

    private async Task EnsureUniqueAsync(
        int CareerId,
        string Code,
        string Version,
        int? ExcludePlanId,
        CancellationToken Cancellation)
    {
        if (await _StudyPlanRepository.ExistsByCodeVersionAsync(
                CareerId,
                Code,
                Version,
                ExcludePlanId,
                true,
                Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "StudyPlans.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code, ["Version"] = Version });
        }
    }

    private async Task EnsureStructureCanChangeAsync(StudyPlan Plan, CancellationToken Cancellation)
    {
        // Un plan con alumnos inscritos conserva su estructura: se crea otra versión
        if (await _StudyPlanRepository.HasStudentsAsync(Plan.Id, Cancellation))
        {
            throw new BusinessRuleException("StudyPlans.InUse");
        }
    }

    // Detección de ciclos en el grafo de prerrequisitos (DFS con estados: 1 = en la pila, 2 = terminado)
    private static bool HasCycle(IReadOnlyList<SubjectPrerequisite> Prerequisites)
    {
        var Graph = Prerequisites
            .GroupBy(Prerequisite => Prerequisite.SubjectId)
            .ToDictionary(
                Group => Group.Key,
                Group => Group.Select(Prerequisite => Prerequisite.PrerequisiteSubjectId).ToList());

        var State = new Dictionary<int, int>();

        bool Visit(int Node)
        {
            if (State.TryGetValue(Node, out var CurrentState))
            {
                if (CurrentState == 1)
                {
                    return true;
                }

                if (CurrentState == 2)
                {
                    return false;
                }
            }

            State[Node] = 1;

            if (Graph.TryGetValue(Node, out var Next))
            {
                foreach (var Neighbor in Next)
                {
                    if (Visit(Neighbor))
                    {
                        return true;
                    }
                }
            }

            State[Node] = 2;
            return false;
        }

        return Graph.Keys.Any(Visit);
    }
}
