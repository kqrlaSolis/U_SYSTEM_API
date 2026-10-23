// Kardex: arma las entradas con materia y grupo a partir de las calificaciones.
using USystem.Application.Dtos;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface ITranscriptService
{
    Task<TranscriptDto> GetByStudentAsync(long StudentId, CancellationToken Cancellation = default);
}

public class TranscriptService : ITranscriptService
{
    private readonly ITranscriptRepository _TranscriptRepository;
    private readonly IClassGroupRepository _GroupRepository;
    private readonly ISubjectRepository _SubjectRepository;

    public TranscriptService(
        ITranscriptRepository TranscriptRepository,
        IClassGroupRepository GroupRepository,
        ISubjectRepository SubjectRepository)
    {
        _TranscriptRepository = TranscriptRepository;
        _GroupRepository = GroupRepository;
        _SubjectRepository = SubjectRepository;
    }

    public async Task<TranscriptDto> GetByStudentAsync(
        long StudentId,
        CancellationToken Cancellation = default)
    {
        var Student = await _TranscriptRepository.GetStudentAsync(StudentId, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        var Rows = await _TranscriptRepository.GetGradesAsync(StudentId, Cancellation);
        var Entries = new List<TranscriptEntryDto>();

        foreach (var Row in Rows)
        {
            var Group = await _GroupRepository.GetByIdAsync(Row.Grade.ClassGroupId, Cancellation);
            var Subject = Group is null
                ? null
                : await _SubjectRepository.GetByIdAsync(Group.SubjectId, Cancellation);

            Entries.Add(new TranscriptEntryDto
            {
                AcademicTermId = Row.Enrollment.AcademicTermId,
                ClassGroupId = Row.Grade.ClassGroupId,
                ClassGroupCode = Group?.Code ?? string.Empty,
                SubjectCode = Subject?.Code ?? string.Empty,
                SubjectName = Subject?.Name ?? string.Empty,
                FinalScore = Row.Grade.FinalScore,
                Status = Row.Grade.Status,
                AttemptNumber = Row.Grade.AttemptNumber,
            });
        }

        var FullName = string.Join(
            ' ',
            new[] { Student.FirstName, Student.PaternalLastName, Student.MaternalLastName }
                .Where(Part => !string.IsNullOrWhiteSpace(Part)));

        return new TranscriptDto
        {
            StudentId = Student.Id,
            StudentNumber = Student.StudentNumber,
            FullName = FullName,
            Entries = Entries,
        };
    }
}
