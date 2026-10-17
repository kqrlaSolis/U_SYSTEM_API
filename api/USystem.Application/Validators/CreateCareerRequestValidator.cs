// Validación del alta de carreras.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class CreateCareerRequestValidator : AbstractValidator<CreateCareerRequest>
{
    public CreateCareerRequestValidator()
    {
        RuleFor(Request => Request.Code).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Code).MaximumLength(20).WithErrorCode("Validation.TooLong");
        RuleFor(Request => Request.Name).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Level).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Modality).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.TermCount).GreaterThan((byte)0).WithErrorCode("Validation.OutOfRange");
        RuleFor(Request => Request.TotalCredits).GreaterThan((byte)0).WithErrorCode("Validation.OutOfRange");
    }
}
