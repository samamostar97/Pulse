using MediatR;
using Pulse.Application.Features.Applications.DTOs;

namespace Pulse.Application.Features.Applications.Commands.RejectApplication;

public class RejectApplicationCommand : IRequest<EventApplicationDto>
{
    public Guid ApplicationId { get; set; }
}
