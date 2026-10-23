// Validación de alta de alumnos.
using FluentValidation;

namespace USystem.Application.Validators;

public class CreateStudentRequestValidator : AbstractValidator<Dtos.CreateStudentRequest>
{
    public CreateStudentRequestValidator()
    {
        RuleFor(Request => Request.CareerId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.StudyPlanId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.EnrollmentTermId).GreaterThan(0).WithMessage("Validation.Required");

        RuleFor(Request => Request.FirstName)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(100).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.PaternalLastName)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(100).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.MaternalLastName).MaximumLength(100).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Curp)
            .NotEmpty().WithMessage("Validation.Required")
            .Length(18).WithMessage("Students.CurpLength");

        RuleFor(Request => Request.Gender)
            .NotEmpty().WithMessage("Validation.Required")
            .Must(Gender => new[] { "M", "F", "Other" }.Contains(Gender))
            .WithMessage("Students.InvalidGender");

        RuleFor(Request => Request.Email)
            .EmailAddress().WithMessage("Validation.InvalidEmail")
            .MaximumLength(150).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Phone).MaximumLength(20).WithMessage("Validation.TooLong");
        RuleFor(Request => Request.PostalCode).MaximumLength(10).WithMessage("Validation.TooLong");
    }
}
