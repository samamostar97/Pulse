using MediatR;
using Pulse.Application.Features.Events.DTOs;

namespace Pulse.Application.Features.Events.Queries.GetEventById;

public class GetEventByIdQuery : IRequest<EventDto>
{
    public Guid EventId { get; set; }
}
