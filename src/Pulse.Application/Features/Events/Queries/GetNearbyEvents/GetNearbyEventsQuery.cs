using MediatR;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.Queries.GetNearbyEvents;

public class GetNearbyEventsQuery : IRequest<PagedResponse<EventDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10;
    public EventCategoryType? Category { get; set; }
    public EventStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
