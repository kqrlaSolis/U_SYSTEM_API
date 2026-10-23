// Propuesta de reinscripción: materias del siguiente cuatrimestre elegibles según seriación.
using USystem.Application.Dtos;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IReEnrollmentService
{
    Task<ReEnrollmentProposalDto> ProposeAsync(long StudentId, CancellationToken Cancellation = default);
}

public class ReEnrollmentService : IReEnrollmentService
{
    private readonly ITranscriptRepository _TranscriptRepository;
    private readonly IStudyPlanRepository _PlanRepository;
    private readonly ISubjectRepository _SubjectRepository;
    private readonly IClassGroupRepository _GroupRepository;

    public ReEnrollmentService(
        ITranscriptRepository TranscriptRepository,
        IStudyPlanRepository PlanRepository,
        ISubjectRepository SubjectRepository,
        IClassGroupRepository GroupRepository)
    {
        _TranscriptRepository = TranscriptRepository;
        _PlanRepository = PlanRepository;
        _SubjectRepository = SubjectRepository;
        _GroupRepository = GroupRepository;
    }

    public async Task<ReEnrollmentProposalDto> ProposeAsync(
        long StudentId,
        CancellationToken Cancellation = default)
    {
        var Student = await _TranscriptRepository.GetStudentAsync(StudentId, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        var PlanSubjects = await _PlanRepository.GetSubjectsAsync(Student.StudyPlanId, Cancellation);
        var Prerequisites = await _PlanRepository.GetPrerequisitesAsync(Student.StudyPlanId, Cancellation);
        var Grades = await _TranscriptRepository.GetGradesAsync(StudentId, Cancellation);

        // Materias aprobadas del alumno (materia del grupo de cada calificación)
        var ApprovedSubjectIds = new HashSet<int>();
        foreach (var Row in Grades)
        {
            if (!string.Equals(Row.Grade.Status, "Passed", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var Group = await _GroupRepository.GetByIdAsync(Row.Grade.ClassGroupId, Cancellation);
            if (Group is not null)
            {
                ApprovedSubjectIds.Add(Group.SubjectId);
            }
        }

        var NextTermNumber = PlanSubjects
            .Where(Item => ApprovedSubjectIds.Contains(Item.SubjectId))
            .Select(Item => (int)Item.TermNumber)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var Subjects = new List<EligibleSubjectDto>();

        foreach (var PlanSubject in PlanSubjects.Where(Item => Item.TermNumber == NextTermNumber))
        {
            var Subject = await _SubjectRepository.GetByIdAsync(PlanSubject.SubjectId, Cancellation);
            if (Subject is null)
            {
                continue;
            }

            var MissingPrerequisite = Prerequisites
                .Where(Item => Item.SubjectId == PlanSubject.SubjectId)
                .Select(Item => Item.PrerequisiteSubjectId)
                .FirstOrDefault(PrerequisiteId => !ApprovedSubjectIds.Contains(PrerequisiteId));

            var IsEligible = MissingPrerequisite == 0;

            Subjects.Add(new EligibleSubjectDto
            {
                SubjectId = Subject.Id,
                Code = Subject.Code,
                Name = Subject.Name,
                Credits = Subject.Credits,
                TermNumber = PlanSubject.TermNumber,
                IsEligible = IsEligible,
                BlockedReason = IsEligible ? null : "ReEnrollments.PendingPrerequisite",
            });
        }

        var FullName = string.Join(
            ' ',
            new[] { Student.FirstName, Student.PaternalLastName, Student.MaternalLastName }
                .Where(Part => !string.IsNullOrWhiteSpace(Part)));

        return new ReEnrollmentProposalDto
        {
            StudentId = Student.Id,
            StudentNumber = Student.StudentNumber,
            FullName = FullName,
            StudyPlanId = Student.StudyPlanId,
            NextTermNumber = NextTermNumber,
            Subjects = Subjects,
        };
    }
}
