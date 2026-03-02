using Pulse.Domain.Enums;

namespace Pulse.Domain.Entities;

public class EventApplication
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public Guid ApplicantId { get; set; }
    public User Applicant { get; set; } = null!;
    public ApplicationStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
