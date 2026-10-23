// Validación de la captura masiva de calificaciones.
using FluentValidation;

namespace USystem.Application.Validators;

public class SaveGradesRequestValidator : AbstractValidator<Dtos.SaveGradesRequest>
{
    public SaveGradesRequestValidator()
    {
        RuleFor(Request => Request.Grades)
            .NotEmpty().WithMessage("Grades.Empty");

        RuleForEach(Request => Request.Grades).ChildRules(Item =>
        {
            Item.RuleFor(Grade => Grade.EnrollmentId).GreaterThan(0).WithMessage("Validation.Required");

            Item.RuleFor(Grade => Grade.Partial1).InclusiveBetween(0, 10).WithMessage("Validation.OutOfRange");
            Item.RuleFor(Grade => Grade.Partial2).InclusiveBetween(0, 10).WithMessage("Validation.OutOfRange");
            Item.RuleFor(Grade => Grade.Partial3).InclusiveBetween(0, 10).WithMessage("Validation.OutOfRange");
            Item.RuleFor(Grade => Grade.Ordinary).InclusiveBetween(0, 10).WithMessage("Validation.OutOfRange");
            Item.RuleFor(Grade => Grade.FinalScore).InclusiveBetween(0, 10).WithMessage("Validation.OutOfRange");

            Item.RuleFor(Grade => Grade.Status)
                .NotEmpty().WithMessage("Validation.Required")
                .Must(Status => new[] { "InProgress", "Passed", "Failed", "NoRight", "NotPresented" }.Contains(Status))
                .WithMessage("Grades.InvalidStatus");
        });
    }
}
