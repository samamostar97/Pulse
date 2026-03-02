using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Domain.Entities;
using Pulse.Domain.Enums;
using Pulse.Domain.ValueObjects;

namespace Pulse.Application.Features.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        var ev = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Location = new Location(request.Latitude, request.Longitude),
            Address = request.Address,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            MaxCapacity = request.MaxCapacity,
            CategoryType = request.CategoryType,
            Status = EventStatus.Upcoming,
            Visibility = request.Visibility,
            AutoApprove = request.AutoApprove,
            ImageUrl = request.ImageUrl,
            OrganizerId = userId
        };

        _context.Events.Add(ev);
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
            ApprovedCount = 0,
            CategoryType = ev.CategoryType,
            Status = ev.Status,
            Visibility = ev.Visibility,
            AutoApprove = ev.AutoApprove,
            ImageUrl = ev.ImageUrl,
            OrganizerId = ev.OrganizerId,
            OrganizerName = $"{user.FirstName} {user.LastName}",
            CreatedAt = ev.CreatedAt
        };
    }
}
