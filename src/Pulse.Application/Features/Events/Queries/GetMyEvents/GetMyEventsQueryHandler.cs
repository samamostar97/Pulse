using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.Queries.GetMyEvents;

public class GetMyEventsQueryHandler : IRequestHandler<GetMyEventsQuery, PagedResponse<EventDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyEventsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResponse<EventDto>> Handle(GetMyEventsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var query = _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.Applications)
            .Where(e => e.OrganizerId == userId)
            .OrderByDescending(e => e.CreatedAt);

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
