// Validación de la propuesta de reinscripción.
using FluentValidation;

namespace USystem.Application.Validators;

public class ProposeReEnrollmentRequestValidator : AbstractValidator<Dtos.ProposeReEnrollmentRequest>
{
    public ProposeReEnrollmentRequestValidator()
    {
        RuleFor(Request => Request.StudentId).GreaterThan(0).WithMessage("Validation.Required");
    }
}
