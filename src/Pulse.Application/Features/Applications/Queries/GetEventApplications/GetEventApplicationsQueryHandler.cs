using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Applications.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Applications.Queries.GetEventApplications;

public class GetEventApplicationsQueryHandler : IRequestHandler<GetEventApplicationsQuery, PagedResponse<EventApplicationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetEventApplicationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResponse<EventApplicationDto>> Handle(GetEventApplicationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var ev = await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Event not found.");

        if (ev.OrganizerId != userId)
            throw new UnauthorizedAccessException("Only the event organizer can view applications.");

        var query = _context.EventApplications
            .AsNoTracking()
            .Include(a => a.Applicant)
            .Include(a => a.Event)
            .Where(a => a.EventId == request.EventId);

        if (request.Status.HasValue)
            query = query.Where(a => a.Status == request.Status.Value);

        query = query.OrderByDescending(a => a.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new EventApplicationDto
            {
                Id = a.Id,
                EventId = a.EventId,
                EventTitle = a.Event.Title,
                ApplicantId = a.ApplicantId,
                ApplicantName = a.Applicant.FirstName + " " + a.Applicant.LastName,
                ApplicantProfileImageUrl = a.Applicant.ProfileImageUrl,
                Status = a.Status,
                Message = a.Message,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<EventApplicationDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
