using FluentValidation;

namespace Pulse.Application.Features.Applications.Commands.WithdrawApplication;

public class WithdrawApplicationCommandValidator : AbstractValidator<WithdrawApplicationCommand>
{
    public WithdrawApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Application ID is required.");
    }
}
