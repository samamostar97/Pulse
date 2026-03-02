using MediatR;

namespace Pulse.Application.Features.Events.Commands.DeleteEvent;

public class DeleteEventCommand : IRequest<Unit>
{
    public Guid EventId { get; set; }
}
