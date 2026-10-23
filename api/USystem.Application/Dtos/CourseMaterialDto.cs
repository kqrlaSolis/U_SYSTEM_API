// Material didáctico de una materia (documento, enlace o video).
namespace USystem.Application.Dtos;

public class CourseMaterialDto
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int? AcademicTermId { get; set; }

    // Syllabus, Reading, Video, Link, File, Other
    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string? FilePath { get; set; }

    public bool IsActive { get; set; }
}

public class CreateCourseMaterialRequest
{
    public int SubjectId { get; set; }

    public int? AcademicTermId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string? FilePath { get; set; }
}

public class UpdateCourseMaterialRequest
{
    public int SubjectId { get; set; }

    public int? AcademicTermId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string? FilePath { get; set; }

    public bool IsActive { get; set; }
}
