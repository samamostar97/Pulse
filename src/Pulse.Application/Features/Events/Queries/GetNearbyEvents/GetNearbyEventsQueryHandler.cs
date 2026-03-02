using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.Queries.GetNearbyEvents;

public class GetNearbyEventsQueryHandler : IRequestHandler<GetNearbyEventsQuery, PagedResponse<EventDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNearbyEventsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<EventDto>> Handle(GetNearbyEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.Applications)
            .AsQueryable();

        if (request.Category.HasValue)
            query = query.Where(e => e.CategoryType == request.Category.Value);

        if (request.Status.HasValue)
            query = query.Where(e => e.Status == request.Status.Value);

        var allEvents = await query.ToListAsync(cancellationToken);

        var nearbyEvents = allEvents
            .Where(e => CalculateDistanceKm(
                request.Latitude, request.Longitude,
                e.Location.Latitude, e.Location.Longitude) <= request.RadiusKm)
            .OrderBy(e => CalculateDistanceKm(
                request.Latitude, request.Longitude,
                e.Location.Latitude, e.Location.Longitude))
            .ToList();

        var totalCount = nearbyEvents.Count;

        var items = nearbyEvents
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
                OrganizerName = $"{e.Organizer.FirstName} {e.Organizer.LastName}",
                CreatedAt = e.CreatedAt
            })
            .ToList();

        return new PagedResponse<EventDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
