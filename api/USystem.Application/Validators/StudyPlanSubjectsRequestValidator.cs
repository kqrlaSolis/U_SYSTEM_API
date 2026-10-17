// Validación de la malla: materias, cuatrimestre y tipo.
using FluentValidation;
using USystem.Application.Dtos;
using USystem.Domain.Constants;

namespace USystem.Application.Validators;

public class StudyPlanSubjectsRequestValidator : AbstractValidator<StudyPlanSubjectsRequest>
{
    public StudyPlanSubjectsRequestValidator()
    {
        RuleFor(Request => Request.Subjects).NotEmpty().WithErrorCode("Validation.Required");

        RuleForEach(Request => Request.Subjects).ChildRules(Subject =>
        {
            Subject.RuleFor(Item => Item.SubjectId).GreaterThan(0).WithErrorCode("Validation.OutOfRange");
            Subject.RuleFor(Item => Item.TermNumber)
                .InclusiveBetween((byte)1, (byte)20)
                .WithErrorCode("Validation.OutOfRange");
            Subject.RuleFor(Item => Item.Type)
                .Must(Type => StudyPlanSubjectTypes.All.Contains(Type))
                .WithErrorCode("Validation.InvalidFormat");
        });
    }
}
