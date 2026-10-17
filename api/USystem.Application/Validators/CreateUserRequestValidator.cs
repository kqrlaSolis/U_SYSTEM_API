// Validación del alta de usuarios; la unicidad y la política se validan en UserService.
using FluentValidation;
using USystem.Application.Dtos;

namespace USystem.Application.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(Request => Request.FirstName).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.PaternalLastName).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Email).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Email).EmailAddress().WithErrorCode("Validation.InvalidEmail");
        RuleFor(Request => Request.Username).NotEmpty().WithErrorCode("Validation.Required");
        RuleFor(Request => Request.Username).MinimumLength(3).WithErrorCode("Validation.InvalidFormat");
        RuleFor(Request => Request.RoleIds).NotEmpty().WithErrorCode("Validation.Required");
    }
}
