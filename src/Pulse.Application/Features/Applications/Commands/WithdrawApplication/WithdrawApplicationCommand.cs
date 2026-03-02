using MediatR;

namespace Pulse.Application.Features.Applications.Commands.WithdrawApplication;

public class WithdrawApplicationCommand : IRequest<Unit>
{
    public Guid ApplicationId { get; set; }
}
