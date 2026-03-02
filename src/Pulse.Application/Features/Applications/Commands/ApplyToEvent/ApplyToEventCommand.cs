using MediatR;
using Pulse.Application.Features.Applications.DTOs;

namespace Pulse.Application.Features.Applications.Commands.ApplyToEvent;

public class ApplyToEventCommand : IRequest<EventApplicationDto>
{
    public Guid EventId { get; set; }
    public string? Message { get; set; }
}
