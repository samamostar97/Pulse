using FluentValidation;

namespace Pulse.Application.Features.Applications.Commands.ApproveApplication;

public class ApproveApplicationCommandValidator : AbstractValidator<ApproveApplicationCommand>
{
    public ApproveApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Application ID is required.");
    }
}
