// Validación del restablecimiento con token.
using FluentValidation;
using USystem.Application.Common;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(Request => Request.Email).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Email).EmailAddress().WithErrorCode("Validation.InvalidEmail");
        RuleFor(Request => Request.Token).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.NewPassword)
            .Must(Password => PasswordPolicy.Validate(Password) is null)
            .WithErrorCode("Validation.PasswordPolicy");
    }
}
