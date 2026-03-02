using MediatR;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.Queries.SearchEvents;

public class SearchEventsQuery : IRequest<PagedResponse<EventDto>>
{
    public string? SearchTerm { get; set; }
    public EventCategoryType? Category { get; set; }
    public EventStatus? Status { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
