// Validación de material didáctico.
using FluentValidation;

namespace USystem.Application.Validators;

public class CreateCourseMaterialRequestValidator : AbstractValidator<Dtos.CreateCourseMaterialRequest>
{
    public CreateCourseMaterialRequestValidator()
    {
        RuleFor(Request => Request.SubjectId).GreaterThan(0).WithMessage("Validation.Required");

        RuleFor(Request => Request.Type)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(20).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Title)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(200).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Url).MaximumLength(500).WithMessage("Validation.TooLong");
        RuleFor(Request => Request.FilePath).MaximumLength(500).WithMessage("Validation.TooLong");
    }
}

public class UpdateCourseMaterialRequestValidator : AbstractValidator<Dtos.UpdateCourseMaterialRequest>
{
    public UpdateCourseMaterialRequestValidator()
    {
        RuleFor(Request => Request.SubjectId).GreaterThan(0).WithMessage("Validation.Required");

        RuleFor(Request => Request.Type)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(20).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Title)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(200).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Url).MaximumLength(500).WithMessage("Validation.TooLong");
        RuleFor(Request => Request.FilePath).MaximumLength(500).WithMessage("Validation.TooLong");
    }
}
