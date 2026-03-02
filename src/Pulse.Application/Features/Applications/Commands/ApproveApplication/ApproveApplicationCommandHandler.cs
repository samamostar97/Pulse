using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Applications.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Applications.Commands.ApproveApplication;

public class ApproveApplicationCommandHandler : IRequestHandler<ApproveApplicationCommand, EventApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ApproveApplicationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EventApplicationDto> Handle(ApproveApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var application = await _context.EventApplications
            .Include(a => a.Event)
            .Include(a => a.Applicant)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (application.Event.OrganizerId != userId)
            throw new UnauthorizedAccessException("Only the event organizer can approve applications.");

        if (application.Status != ApplicationStatus.Pending)
            throw new InvalidOperationException("Only pending applications can be approved.");

        application.Status = ApplicationStatus.Approved;
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
