// Validación del cambio de contraseña propio.
using FluentValidation;
using USystem.Application.Common;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(Request => Request.CurrentPassword)
            .NotEmpty()
            .WithErrorCode("Validation.Required");

        RuleFor(Request => Request.NewPassword)
            .Must(Password => PasswordPolicy.Validate(Password) is null)
            .WithErrorCode("Validation.PasswordPolicy");

        RuleFor(Request => Request.NewPassword)
            .NotEqual(Request => Request.CurrentPassword)
            .WithErrorCode("Auth.PasswordReused");
    }
}
