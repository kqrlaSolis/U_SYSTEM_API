// Validación de forma del login; las reglas de negocio viven en AuthService.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(Request => Request.UsernameOrEmail)
            .NotEmpty()
            .WithErrorCode("Validation.Required");

        RuleFor(Request => Request.Password)
            .NotEmpty()
            .WithErrorCode("Validation.Required");
    }
}
