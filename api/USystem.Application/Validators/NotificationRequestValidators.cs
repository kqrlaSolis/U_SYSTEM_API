// Validación de plantillas y envíos de notificación.
using FluentValidation;

namespace USystem.Application.Validators;

public class CreateNotificationTemplateRequestValidator : AbstractValidator<Dtos.CreateNotificationTemplateRequest>
{
    public CreateNotificationTemplateRequestValidator()
    {
        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(50).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.SubjectLine)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(200).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Body)
            .NotEmpty().WithMessage("Validation.Required");
    }
}

public class UpdateNotificationTemplateRequestValidator : AbstractValidator<Dtos.UpdateNotificationTemplateRequest>
{
    public UpdateNotificationTemplateRequestValidator()
    {
        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(50).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.SubjectLine)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(200).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Body)
            .NotEmpty().WithMessage("Validation.Required");
    }
}

public class SendNotificationRequestValidator : AbstractValidator<Dtos.SendNotificationRequest>
{
    public SendNotificationRequestValidator()
    {
        RuleFor(Request => Request.TemplateCode)
            .NotEmpty().WithMessage("Validation.Required");

        RuleFor(Request => Request.RecipientEmail)
            .NotEmpty().WithMessage("Validation.Required")
            .EmailAddress().WithMessage("Validation.InvalidEmail");
    }
}
