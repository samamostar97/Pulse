using MediatR;
using Pulse.Application.Features.Applications.DTOs;

namespace Pulse.Application.Features.Applications.Commands.ApproveApplication;

public class ApproveApplicationCommand : IRequest<EventApplicationDto>
{
    public Guid ApplicationId { get; set; }
}
