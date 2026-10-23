// Validación de grupos y horarios.
using FluentValidation;

namespace USystem.Application.Validators;

public class CreateClassGroupRequestValidator : AbstractValidator<Dtos.CreateClassGroupRequest>
{
    public CreateClassGroupRequestValidator()
    {
        RuleFor(Request => Request.AcademicTermId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.SubjectId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.TeacherId).GreaterThan(0).WithMessage("Validation.Required");

        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(20).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Capacity).GreaterThan((short)0).WithMessage("ClassGroups.InvalidCapacity");
        RuleFor(Request => Request.Classroom).MaximumLength(50).WithMessage("Validation.TooLong");

        RuleForEach(Request => Request.Schedules).ChildRules(Schedule =>
        {
            Schedule.RuleFor(Item => Item.DayOfWeek)
                .InclusiveBetween(1, 7).WithMessage("ClassGroups.InvalidSchedule");
            Schedule.RuleFor(Item => Item.StartTime)
                .NotEmpty().Matches("^([01]\\d|2[0-3]):[0-5]\\d$").WithMessage("ClassGroups.InvalidSchedule");
            Schedule.RuleFor(Item => Item.EndTime)
                .NotEmpty().Matches("^([01]\\d|2[0-3]):[0-5]\\d$").WithMessage("ClassGroups.InvalidSchedule");
            Schedule.RuleFor(Item => Item)
                .Must(Item => Item.StartTime.CompareTo(Item.EndTime) < 0)
                .WithMessage("ClassGroups.InvalidSchedule");
        });
    }
}

public class UpdateClassGroupRequestValidator : AbstractValidator<Dtos.UpdateClassGroupRequest>
{
    public UpdateClassGroupRequestValidator()
    {
        RuleFor(Request => Request.AcademicTermId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.SubjectId).GreaterThan(0).WithMessage("Validation.Required");
        RuleFor(Request => Request.TeacherId).GreaterThan(0).WithMessage("Validation.Required");

        RuleFor(Request => Request.Code)
            .NotEmpty().WithMessage("Validation.Required")
            .MaximumLength(20).WithMessage("Validation.TooLong");

        RuleFor(Request => Request.Capacity).GreaterThan((short)0).WithMessage("ClassGroups.InvalidCapacity");
        RuleFor(Request => Request.Classroom).MaximumLength(50).WithMessage("Validation.TooLong");

        RuleForEach(Request => Request.Schedules).ChildRules(Schedule =>
        {
            Schedule.RuleFor(Item => Item.DayOfWeek)
                .InclusiveBetween(1, 7).WithMessage("ClassGroups.InvalidSchedule");
            Schedule.RuleFor(Item => Item.StartTime)
                .NotEmpty().Matches("^([01]\\d|2[0-3]):[0-5]\\d$").WithMessage("ClassGroups.InvalidSchedule");
            Schedule.RuleFor(Item => Item.EndTime)
                .NotEmpty().Matches("^([01]\\d|2[0-3]):[0-5]\\d$").WithMessage("ClassGroups.InvalidSchedule");
            Schedule.RuleFor(Item => Item)
                .Must(Item => Item.StartTime.CompareTo(Item.EndTime) < 0)
                .WithMessage("ClassGroups.InvalidSchedule");
        });
    }
}
