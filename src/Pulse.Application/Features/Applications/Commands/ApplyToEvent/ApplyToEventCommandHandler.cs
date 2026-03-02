using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Applications.DTOs;
using Pulse.Domain.Entities;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Applications.Commands.ApplyToEvent;

public class ApplyToEventCommandHandler : IRequestHandler<ApplyToEventCommand, EventApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ApplyToEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EventApplicationDto> Handle(ApplyToEventCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var ev = await _context.Events
            .Include(e => e.Applications)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Event not found.");

        if (ev.OrganizerId == userId)
            throw new InvalidOperationException("You cannot apply to your own event.");

        if (ev.Status != EventStatus.Upcoming)
            throw new InvalidOperationException("Applications are only accepted for upcoming events.");

        var existingApplication = ev.Applications
            .FirstOrDefault(a => a.ApplicantId == userId);

        if (existingApplication != null)
            throw new InvalidOperationException("You have already applied to this event.");

        var approvedCount = ev.Applications.Count(a => a.Status == ApplicationStatus.Approved);
        if (approvedCount >= ev.MaxCapacity)
            throw new InvalidOperationException("This event has reached its maximum capacity.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        var application = new EventApplication
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            ApplicantId = userId,
            Message = request.Message,
            Status = ev.AutoApprove ? ApplicationStatus.Approved : ApplicationStatus.Pending
        };

        _context.EventApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return new EventApplicationDto
        {
            Id = application.Id,
            EventId = application.EventId,
            EventTitle = ev.Title,
            ApplicantId = userId,
            ApplicantName = $"{user.FirstName} {user.LastName}",
            ApplicantProfileImageUrl = user.ProfileImageUrl,
            Status = application.Status,
            Message = application.Message,
            CreatedAt = application.CreatedAt
        };
    }
}
