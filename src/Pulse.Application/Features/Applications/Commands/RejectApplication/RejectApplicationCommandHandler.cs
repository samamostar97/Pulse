using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Applications.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Applications.Commands.RejectApplication;

public class RejectApplicationCommandHandler : IRequestHandler<RejectApplicationCommand, EventApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RejectApplicationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EventApplicationDto> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var application = await _context.EventApplications
            .Include(a => a.Event)
            .Include(a => a.Applicant)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (application.Event.OrganizerId != userId)
            throw new UnauthorizedAccessException("Only the event organizer can reject applications.");

        if (application.Status != ApplicationStatus.Pending)
            throw new InvalidOperationException("Only pending applications can be rejected.");

        application.Status = ApplicationStatus.Rejected;
        await _context.SaveChangesAsync(cancellationToken);

        return new EventApplicationDto
        {
            Id = application.Id,
            EventId = application.EventId,
            EventTitle = application.Event.Title,
            ApplicantId = application.ApplicantId,
            ApplicantName = $"{application.Applicant.FirstName} {application.Applicant.LastName}",
            ApplicantProfileImageUrl = application.Applicant.ProfileImageUrl,
            Status = application.Status,
            Message = application.Message,
            CreatedAt = application.CreatedAt
        };
    }
}
