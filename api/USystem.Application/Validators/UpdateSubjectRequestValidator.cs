// Validación de la edición de materias.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>
{
    public UpdateSubjectRequestValidator()
    {
        RuleFor(Request => Request.Code).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Code).MaximumLength(20).WithErrorCode("Validation.TooLong");
        RuleFor(Request => Request.Name).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Credits).GreaterThan((byte)0).WithErrorCode("Validation.OutOfRange");
    }
}
