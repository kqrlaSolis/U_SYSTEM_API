// Grupos de clase con sus horarios.
using System.Globalization;

namespace USystem.Application.Dtos;

public class ClassGroupScheduleDto
{
    // 1 = lunes … 7 = domingo
    public int DayOfWeek { get; set; }

    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;
}

public class ClassGroupDto
{
    public int Id { get; set; }

    public int AcademicTermId { get; set; }

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string? Classroom { get; set; }

    public short Capacity { get; set; }

    public short EnrolledCount { get; set; }

    // Open, Closed, Cancelled
    public string Status { get; set; } = string.Empty;

    public List<ClassGroupScheduleDto> Schedules { get; set; } = new();
}

public class ClassGroupScheduleRequest
{
    public int DayOfWeek { get; set; }

    // HH:mm
    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;

    public static TimeOnly ParseTime(string Value)
        => TimeOnly.ParseExact(Value.Trim(), "HH:mm", CultureInfo.InvariantCulture);
}

public class CreateClassGroupRequest
{
    public int AcademicTermId { get; set; }

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string? Classroom { get; set; }

    public short Capacity { get; set; }

    public string Status { get; set; } = "Open";

    public List<ClassGroupScheduleRequest> Schedules { get; set; } = new();
}

public class UpdateClassGroupRequest
{
    public int AcademicTermId { get; set; }

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string? Classroom { get; set; }

    public short Capacity { get; set; }

    public string Status { get; set; } = "Open";

    public List<ClassGroupScheduleRequest> Schedules { get; set; } = new();
}
