// Validación de la edición de usuarios.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(Request => Request.FirstName).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.PaternalLastName).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Email).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Email).EmailAddress().WithErrorCode("Validation.InvalidEmail");
        RuleFor(Request => Request.Username).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Username).MinimumLength(3).WithErrorCode("Validation.InvalidFormat");
    }
}
