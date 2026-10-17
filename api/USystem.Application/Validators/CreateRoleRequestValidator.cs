// Validación de los roles: el nombre es obligatorio y los permisos son opcionales.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(Request => Request.Name).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Name).MaximumLength(50).WithErrorCode("Validation.TooLong");
    }
}
