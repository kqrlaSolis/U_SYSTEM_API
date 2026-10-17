// Validación del alta de periodos: formato de clave, fechas obligatorias y orden de cada rango.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class CreateAcademicTermRequestValidator : AbstractValidator<CreateAcademicTermRequest>
{
    public CreateAcademicTermRequestValidator()
    {
        RuleFor(Request => Request.Code)
            .NotEmpty()
            .WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Code)
            .Matches(@"^\d{4}-[1-3]$")
            .WithErrorCode("Validation.InvalidFormat");

        RuleFor(Request => Request.Name).NotEmpty().WithErrorCode("Validation.Required");

        RuleFor(Request => Request.AcademicYear)
            .GreaterThan((short)1999)
            .WithErrorCode("Validation.OutOfRange");
        RuleFor(Request => Request.TermNumber)
            .InclusiveBetween((byte)1, (byte)3)
            .WithErrorCode("Validation.OutOfRange");

        RuleFor(Request => Request.StartDate).Must(Date => Date != default).WithErrorCode("Validation.Required");
        RuleFor(Request => Request.EndDate).Must(Date => Date != default).WithErrorCode("Validation.Required");
        RuleFor(Request => Request.EnrollmentStartDate)
            .Must(Date => Date != default)
            .WithErrorCode("Validation.Required");
        RuleFor(Request => Request.EnrollmentEndDate)
            .Must(Date => Date != default)
            .WithErrorCode("Validation.Required");
        RuleFor(Request => Request.ReEnrollmentStartDate)
            .Must(Date => Date != default)
            .WithErrorCode("Validation.Required");
        RuleFor(Request => Request.ReEnrollmentEndDate)
            .Must(Date => Date != default)
            .WithErrorCode("Validation.Required");
        RuleFor(Request => Request.ExamStartDate).Must(Date => Date != default).WithErrorCode("Validation.Required");
        RuleFor(Request => Request.ExamEndDate).Must(Date => Date != default).WithErrorCode("Validation.Required");
        RuleFor(Request => Request.GradeReportCloseDate)
            .Must(Date => Date != default)
            .WithErrorCode("Validation.Required");

        RuleFor(Request => Request)
            .Must(Request => Request.StartDate < Request.EndDate)
            .WithErrorCode("AcademicTerms.InvalidDates");
        RuleFor(Request => Request)
            .Must(Request => Request.EnrollmentStartDate <= Request.EnrollmentEndDate)
            .WithErrorCode("AcademicTerms.InvalidDates");
        RuleFor(Request => Request)
            .Must(Request => Request.ReEnrollmentStartDate <= Request.ReEnrollmentEndDate)
            .WithErrorCode("AcademicTerms.InvalidDates");
        RuleFor(Request => Request)
            .Must(Request => Request.ExamStartDate <= Request.ExamEndDate)
            .WithErrorCode("AcademicTerms.InvalidDates");
    }
}
