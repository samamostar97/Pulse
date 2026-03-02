using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Applications.DTOs;

public class EventApplicationDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public Guid ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string? ApplicantProfileImageUrl { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}
