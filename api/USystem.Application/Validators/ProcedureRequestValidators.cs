// Validación de tipos y solicitudes de trámite.
using FluentValidation;

namespace USystem.Application.Validators;

public class CreateProcedureTypeRequestValidator : AbstractValidator<Dtos.CreateProcedureTypeRequest>
{
    public CreateProcedureTypeRequestValidator()
    {
        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(50).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Name)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(150).WithMessage("Validation.TooLong");
    }
}

public class UpdateProcedureTypeRequestValidator : AbstractValidator<Dtos.UpdateProcedureTypeRequest>
{
    public UpdateProcedureTypeRequestValidator()
    {
        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(50).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Name)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(150).WithMessage("Validation.TooLong");
    }
}

public class CreateProcedureRequestValidator : AbstractValidator<Dtos.CreateProcedureRequest>
{
    public CreateProcedureRequestValidator()
    {
        RuleFor(Request => Request.ProcedureTypeId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.StudentId).GreaterThan(0).WithMessage("Validation.Required");
    }
}
