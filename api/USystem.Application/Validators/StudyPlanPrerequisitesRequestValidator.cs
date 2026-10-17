// Validación de la seriación: sin autorreferencias ni valores vacíos.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class StudyPlanPrerequisitesRequestValidator : AbstractValidator<StudyPlanPrerequisitesRequest>
{
    public StudyPlanPrerequisitesRequestValidator()
    {
        RuleForEach(Request => Request.Prerequisites).ChildRules(Prerequisite =>
        {
            Prerequisite.RuleFor(Item => Item.SubjectId)
                .GreaterThan(0)
                .WithErrorCode("Validation.OutOfRange");
            Prerequisite.RuleFor(Item => Item.PrerequisiteSubjectId)
                .GreaterThan(0)
                .WithErrorCode("Validation.OutOfRange");
            Prerequisite.RuleFor(Item => Item)
                .Must(Item => Item.SubjectId != Item.PrerequisiteSubjectId)
                .WithErrorCode("Validation.InvalidFormat");
        });
    }
}
