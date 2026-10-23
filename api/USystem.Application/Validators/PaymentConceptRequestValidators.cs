// Validación de conceptos de pago.
using FluentValidation;

namespace USystem.Application.Validators;

public class CreatePaymentConceptRequestValidator : AbstractValidator<Dtos.CreatePaymentConceptRequest>
{
    public CreatePaymentConceptRequestValidator()
    {
        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(50).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Name)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(150).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.BaseAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Validation.OutOfRange");
    }
}

public class UpdatePaymentConceptRequestValidator : AbstractValidator<Dtos.UpdatePaymentConceptRequest>
{
    public UpdatePaymentConceptRequestValidator()
    {
        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(50).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Name)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(150).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.BaseAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Validation.OutOfRange");
    }
}
