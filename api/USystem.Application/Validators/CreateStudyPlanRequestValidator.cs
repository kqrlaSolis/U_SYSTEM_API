// Validación del alta de planes de estudio.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class CreateStudyPlanRequestValidator : AbstractValidator<CreateStudyPlanRequest>
{
    public CreateStudyPlanRequestValidator()
    {
        RuleFor(Request => Request.CareerId).GreaterThan(0).WithErrorCode("Validation.OutOfRange");
        RuleFor(Request => Request.Code).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Code).MaximumLength(20).WithErrorCode("Validation.TooLong");
        RuleFor(Request => Request.Version).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Version).MaximumLength(20).WithErrorCode("Validation.TooLong");
        RuleFor(Request => Request.StartDate).Must(Date => Date != default).WithErrorCode("Validation.Required");
        RuleFor(Request => Request.TotalCredits).GreaterThan((short)0).WithErrorCode("Validation.OutOfRange");
        RuleFor(Request => Request)
            .Must(Request => Request.EndDate is null || Request.EndDate >= Request.StartDate)
            .WithErrorCode("AcademicTerms.InvalidDates");
    }
}
