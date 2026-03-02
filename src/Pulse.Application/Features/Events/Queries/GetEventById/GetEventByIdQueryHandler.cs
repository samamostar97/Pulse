using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.Queries.GetEventById;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IApplicationDbContext _context;

    public GetEventByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.Applications)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Event not found.");

        return new EventDto
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            Latitude = ev.Location.Latitude,
            Longitude = ev.Location.Longitude,
            Address = ev.Address,
            StartsAt = ev.StartsAt,
            EndsAt = ev.EndsAt,
            MaxCapacity = ev.MaxCapacity,
            ApprovedCount = ev.Applications.Count(a => a.Status == ApplicationStatus.Approved),
            CategoryType = ev.CategoryType,
            Status = ev.Status,
            Visibility = ev.Visibility,
            AutoApprove = ev.AutoApprove,
            ImageUrl = ev.ImageUrl,
            OrganizerId = ev.OrganizerId,
            OrganizerName = $"{ev.Organizer.FirstName} {ev.Organizer.LastName}",
            CreatedAt = ev.CreatedAt
        };
    }
}
