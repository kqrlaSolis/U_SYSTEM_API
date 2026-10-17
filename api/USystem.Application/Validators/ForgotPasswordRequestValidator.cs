// Validación de la solicitud de recuperación.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(Request => Request.Email).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Email).EmailAddress().WithErrorCode("Validation.InvalidEmail");
    }
}
