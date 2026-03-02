using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.Queries.SearchEvents;

public class SearchEventsQueryHandler : IRequestHandler<SearchEventsQuery, PagedResponse<EventDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchEventsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<EventDto>> Handle(SearchEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.Applications)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(e =>
                e.Title.ToLower().Contains(term) ||
                e.Description.ToLower().Contains(term));
        }

        if (request.Category.HasValue)
            query = query.Where(e => e.CategoryType == request.Category.Value);

        if (request.Status.HasValue)
            query = query.Where(e => e.Status == request.Status.Value);

        query = request.SortBy?.ToLower() switch
        {
            "startsat" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(e => e.StartsAt)
                : query.OrderByDescending(e => e.StartsAt),
            "createdat" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(e => e.CreatedAt)
                : query.OrderByDescending(e => e.CreatedAt),
            "title" => request.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(e => e.Title)
                : query.OrderByDescending(e => e.Title),
            _ => query.OrderByDescending(e => e.StartsAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Latitude = e.Location.Latitude,
                Longitude = e.Location.Longitude,
                Address = e.Address,
                StartsAt = e.StartsAt,
                EndsAt = e.EndsAt,
                MaxCapacity = e.MaxCapacity,
                ApprovedCount = e.Applications.Count(a => a.Status == ApplicationStatus.Approved),
                CategoryType = e.CategoryType,
                Status = e.Status,
                Visibility = e.Visibility,
                AutoApprove = e.AutoApprove,
                ImageUrl = e.ImageUrl,
                OrganizerId = e.OrganizerId,
                OrganizerName = e.Organizer.FirstName + " " + e.Organizer.LastName,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<EventDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
