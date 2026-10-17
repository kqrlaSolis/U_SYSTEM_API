// Tipos de materia dentro de un plan de estudios.
namespace USystem.Domain.Constants;

public static class StudyPlanSubjectTypes
{
    public const string Mandatory = "Mandatory";
    public const string Elective = "Elective";

    public static readonly IReadOnlyList<string> All = new[] { Mandatory, Elective };
}
