using FluentValidation;

namespace DevLife.Backend.Modules.Auth;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).MaximumLength(20);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");

        RuleFor(x => x.Stack)
            .NotEmpty().WithMessage("Tech stack is required");

        RuleFor(x => x.Experience)
            .NotEmpty().WithMessage("Experience is required");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("Birth date must be in the past");
    }
}
