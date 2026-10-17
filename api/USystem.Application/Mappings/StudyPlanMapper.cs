// Mapeo de planes de estudio, su malla y su seriación.
using USystem.Application.Dtos;
using USystem.Domain.Entities;

namespace USystem.Application.Mappings;

public static class StudyPlanMapper
{
    public static StudyPlanDto ToDto(
        StudyPlan Plan,
        string CareerName,
        IReadOnlyDictionary<int, Subject> SubjectsById,
        IReadOnlyList<StudyPlanSubject> PlanSubjects,
        IReadOnlyList<SubjectPrerequisite> Prerequisites) => new()
        {
            Id = Plan.Id,
            CareerId = Plan.CareerId,
            CareerName = CareerName,
            Code = Plan.Code,
            Version = Plan.Version,
            StartDate = Plan.StartDate,
            EndDate = Plan.EndDate,
            TotalCredits = Plan.TotalCredits,
            AssignedCredits = SumCredits(PlanSubjects, SubjectsById),
            IsCurrent = Plan.IsCurrent,
            Subjects = PlanSubjects
            .OrderBy(PlanSubject => PlanSubject.TermNumber)
            .ThenBy(PlanSubject => PlanSubject.SubjectId)
            .Select(PlanSubject => new StudyPlanSubjectDto
            {
                SubjectId = PlanSubject.SubjectId,
                SubjectCode = SubjectsById.TryGetValue(PlanSubject.SubjectId, out var Subject) ? Subject.Code : string.Empty,
                SubjectName = Subject?.Name ?? string.Empty,
                Credits = Subject?.Credits ?? 0,
                TermNumber = PlanSubject.TermNumber,
                Type = PlanSubject.Type,
            })
            .ToList(),
            Prerequisites = Prerequisites
            .OrderBy(Prerequisite => Prerequisite.SubjectId)
            .Select(Prerequisite => new SubjectPrerequisiteDto
            {
                SubjectId = Prerequisite.SubjectId,
                SubjectCode = SubjectsById.TryGetValue(Prerequisite.SubjectId, out var Subject) ? Subject.Code : string.Empty,
                PrerequisiteSubjectId = Prerequisite.PrerequisiteSubjectId,
                PrerequisiteSubjectCode = SubjectsById.TryGetValue(Prerequisite.PrerequisiteSubjectId, out var Required)
                    ? Required.Code
                    : string.Empty,
            })
            .ToList(),
        };

    public static short SumCredits(
        IReadOnlyList<StudyPlanSubject> PlanSubjects,
        IReadOnlyDictionary<int, Subject> SubjectsById)
        => (short)PlanSubjects
            .Select(PlanSubject => PlanSubject.SubjectId)
            .Distinct()
            .Where(SubjectsById.ContainsKey)
            .Sum(SubjectId => (int)SubjectsById[SubjectId].Credits);
}
