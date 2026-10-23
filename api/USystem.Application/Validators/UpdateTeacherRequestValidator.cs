// Validación de edición de docentes.
using FluentValidation;

namespace USystem.Application.Validators;

public class UpdateTeacherRequestValidator : AbstractValidator<Dtos.UpdateTeacherRequest>
{
    public UpdateTeacherRequestValidator()
    {
        RuleFor(Request => Request.EmployeeNumber)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(20).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.FirstName)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(100).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.PaternalLastName)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(100).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.MaternalLastName)
            .MaximumLength(100).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Email)
            .NotEmpty().WithMessage("Validation.Required")
            .EmailAddress().WithMessage("Validation.InvalidEmail")
            .MaximumLength(150).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Phone)
            .MaximumLength(20).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Specialty)
            .MaximumLength(150).WithMessage("Validation.TooLong");
    }
}
