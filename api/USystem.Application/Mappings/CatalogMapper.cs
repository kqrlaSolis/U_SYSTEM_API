// Mapeo de entidades del catálogo académico a DTOs.
using USystem.Application.Dtos;
using USystem.Domain.Entities;

namespace USystem.Application.Mappings;

public static class CatalogMapper
{
    public static CareerDto ToDto(Career Career) => new()
    {
        Id = Career.Id,
        Code = Career.Code,
        Name = Career.Name,
        Level = Career.Level,
        TermCount = Career.TermCount,
        TotalCredits = Career.TotalCredits,
        Modality = Career.Modality,
        IsActive = Career.IsActive,
    };

    public static SubjectDto ToDto(Subject Subject) => new()
    {
        Id = Subject.Id,
        Code = Subject.Code,
        Name = Subject.Name,
        Credits = Subject.Credits,
        TheoryHours = Subject.TheoryHours,
        PracticeHours = Subject.PracticeHours,
        IsActive = Subject.IsActive,
    };

    public static AcademicTermDto ToDto(AcademicTerm Term) => new()
    {
        Id = Term.Id,
        Code = Term.Code,
        Name = Term.Name,
        AcademicYear = Term.AcademicYear,
        TermNumber = Term.TermNumber,
        StartDate = Term.StartDate,
        EndDate = Term.EndDate,
        EnrollmentStartDate = Term.EnrollmentStartDate,
        EnrollmentEndDate = Term.EnrollmentEndDate,
        ReEnrollmentStartDate = Term.ReEnrollmentStartDate,
        ReEnrollmentEndDate = Term.ReEnrollmentEndDate,
        ExamStartDate = Term.ExamStartDate,
        ExamEndDate = Term.ExamEndDate,
        GradeReportCloseDate = Term.GradeReportCloseDate,
        Status = Term.Status,
    };
}
