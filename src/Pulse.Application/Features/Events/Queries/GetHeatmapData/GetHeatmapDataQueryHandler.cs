using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.Queries.GetHeatmapData;

public class GetHeatmapDataQueryHandler : IRequestHandler<GetHeatmapDataQuery, List<HeatmapPointDto>>
{
    private readonly IApplicationDbContext _context;

    public GetHeatmapDataQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HeatmapPointDto>> Handle(GetHeatmapDataQuery request, CancellationToken cancellationToken)
    {
        var events = await _context.Events
            .AsNoTracking()
            .Include(e => e.Applications)
            .Where(e => e.Status == EventStatus.Upcoming || e.Status == EventStatus.Ongoing)
            .ToListAsync(cancellationToken);

        var nearby = events
            .Where(e => CalculateDistanceKm(
                request.Latitude, request.Longitude,
                e.Location.Latitude, e.Location.Longitude) <= request.RadiusKm)
            .ToList();

        var now = DateTime.UtcNow;

        return nearby.Select(e =>
        {
            var approvedCount = e.Applications.Count(a => a.Status == ApplicationStatus.Approved);
            var capacityFilled = e.MaxCapacity > 0 ? (double)approvedCount / e.MaxCapacity : 0;
            var startsWithin24h = (e.StartsAt - now).TotalHours <= 24 && e.StartsAt > now;

            var weight = 1.0
                + Math.Min(approvedCount * 0.5, 5.0)
                + (startsWithin24h ? 2.0 : 0.0)
                + (capacityFilled >= 0.8 ? 1.0 : 0.0);

            return new HeatmapPointDto
            {
                Latitude = e.Location.Latitude,
                Longitude = e.Location.Longitude,
                Weight = weight,
                EventId = e.Id
            };
        }).ToList();
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
