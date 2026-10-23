// Avance académico: créditos aprobados contra los créditos del plan.
using USystem.Application.Dtos;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IAcademicProgressService
{
    Task<AcademicProgressDto> GetByStudentAsync(long StudentId, CancellationToken Cancellation = default);
}

public class AcademicProgressService : IAcademicProgressService
{
    private readonly ITranscriptRepository _TranscriptRepository;
    private readonly IStudyPlanRepository _StudyPlanRepository;
    private readonly IClassGroupRepository _GroupRepository;
    private readonly ISubjectRepository _SubjectRepository;

    public AcademicProgressService(
        ITranscriptRepository TranscriptRepository,
        IStudyPlanRepository StudyPlanRepository,
        IClassGroupRepository GroupRepository,
        ISubjectRepository SubjectRepository)
    {
        _TranscriptRepository = TranscriptRepository;
        _StudyPlanRepository = StudyPlanRepository;
        _GroupRepository = GroupRepository;
        _SubjectRepository = SubjectRepository;
    }

    public async Task<AcademicProgressDto> GetByStudentAsync(
        long StudentId,
        CancellationToken Cancellation = default)
    {
        var Student = await _TranscriptRepository.GetStudentAsync(StudentId, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        var Plan = await _StudyPlanRepository.GetByIdAsync(Student.StudyPlanId, Cancellation)
            ?? throw new NotFoundException("StudyPlans.NotFound");

        var Rows = await _TranscriptRepository.GetGradesAsync(StudentId, Cancellation);

        short ApprovedCredits = 0;
        var PassedSubjects = new HashSet<int>();
        var FailedSubjects = new HashSet<int>();

        foreach (var Row in Rows)
        {
            var Group = await _GroupRepository.GetByIdAsync(Row.Grade.ClassGroupId, Cancellation);
            if (Group is null)
            {
                continue;
            }

            var Subject = await _SubjectRepository.GetByIdAsync(Group.SubjectId, Cancellation);
            if (Subject is null)
            {
                continue;
            }

            if (string.Equals(Row.Grade.Status, "Passed", StringComparison.OrdinalIgnoreCase))
            {
                if (PassedSubjects.Add(Subject.Id))
                {
                    ApprovedCredits = (short)(ApprovedCredits + Subject.Credits);
                }
            }
            else if (string.Equals(Row.Grade.Status, "Failed", StringComparison.OrdinalIgnoreCase))
            {
                FailedSubjects.Add(Subject.Id);
            }
        }

        var ProgressPercent = Plan.TotalCredits == 0
            ? 0m
            : Math.Round(ApprovedCredits * 100m / Plan.TotalCredits, 2);

        var FullName = string.Join(
            ' ',
            new[] { Student.FirstName, Student.PaternalLastName, Student.MaternalLastName }
                .Where(Part => !string.IsNullOrWhiteSpace(Part)));

        return new AcademicProgressDto
        {
            StudentId = Student.Id,
            StudentNumber = Student.StudentNumber,
            FullName = FullName,
            StudyPlanId = Plan.Id,
            PlanTotalCredits = Plan.TotalCredits,
            ApprovedCredits = ApprovedCredits,
            ProgressPercent = ProgressPercent,
            PassedSubjects = PassedSubjects.Count,
            FailedSubjects = FailedSubjects.Count,
        };
    }
}
