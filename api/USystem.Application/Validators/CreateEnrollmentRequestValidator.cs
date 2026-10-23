// Validación de inscripciones.
using FluentValidation;

namespace USystem.Application.Validators;

public class CreateEnrollmentRequestValidator : AbstractValidator<Dtos.CreateEnrollmentRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(Request => Request.StudentId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.AcademicTermId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.CareerId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.Notes).MaximumLength(500).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.ClassGroupIds)
            .NotEmpty().WithMessage("Enrollments.ClassGroupsRequired");
    }
}
