using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Enums;
using Pulse.Domain.ValueObjects;

namespace Pulse.Application.Features.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, EventDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EventDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var ev = await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Applications)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Event not found.");

        if (ev.OrganizerId != userId)
            throw new UnauthorizedAccessException("Only the event organizer can update this event.");

        ev.Title = request.Title;
        ev.Description = request.Description;
        ev.Location = new Location(request.Latitude, request.Longitude);
        ev.Address = request.Address;
        ev.StartsAt = request.StartsAt;
        ev.EndsAt = request.EndsAt;
        ev.MaxCapacity = request.MaxCapacity;
        ev.CategoryType = request.CategoryType;
        ev.Visibility = request.Visibility;
        ev.AutoApprove = request.AutoApprove;
        ev.ImageUrl = request.ImageUrl;

        await _context.SaveChangesAsync(cancellationToken);

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
