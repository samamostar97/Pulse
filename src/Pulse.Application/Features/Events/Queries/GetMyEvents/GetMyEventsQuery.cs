using MediatR;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Events.DTOs;

namespace Pulse.Application.Features.Events.Queries.GetMyEvents;

public class GetMyEventsQuery : IRequest<PagedResponse<EventDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
