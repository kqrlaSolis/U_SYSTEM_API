// Kardex de un alumno agrupado por periodo.
namespace USystem.Application.Dtos;

public class TranscriptEntryDto
{
    public int AcademicTermId { get; set; }

    public int ClassGroupId { get; set; }

    public string ClassGroupCode { get; set; } = string.Empty;

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public decimal? FinalScore { get; set; }

    public string Status { get; set; } = string.Empty;

    public byte AttemptNumber { get; set; }
}

public class TranscriptDto
{
    public long StudentId { get; set; }

    public string StudentNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public List<TranscriptEntryDto> Entries { get; set; } = new();
}
