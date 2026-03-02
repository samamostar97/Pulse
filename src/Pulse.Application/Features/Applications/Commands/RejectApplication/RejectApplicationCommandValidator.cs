using FluentValidation;

namespace Pulse.Application.Features.Applications.Commands.RejectApplication;

public class RejectApplicationCommandValidator : AbstractValidator<RejectApplicationCommand>
{
    public RejectApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Application ID is required.");
    }
}
