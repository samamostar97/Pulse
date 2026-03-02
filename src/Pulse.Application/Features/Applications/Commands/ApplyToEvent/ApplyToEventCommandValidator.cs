using FluentValidation;

namespace Pulse.Application.Features.Applications.Commands.ApplyToEvent;

public class ApplyToEventCommandValidator : AbstractValidator<ApplyToEventCommand>
{
    public ApplyToEventCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.Message)
            .MaximumLength(500).When(x => x.Message != null);
    }
}
