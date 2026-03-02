using MediatR;
using Pulse.Application.Common.Models;
using Pulse.Application.Features.Applications.DTOs;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Applications.Queries.GetEventApplications;

public class GetEventApplicationsQuery : IRequest<PagedResponse<EventApplicationDto>>
{
    public Guid EventId { get; set; }
    public ApplicationStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
