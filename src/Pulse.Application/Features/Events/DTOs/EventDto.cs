using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.DTOs;

public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int MaxCapacity { get; set; }
    public int ApprovedCount { get; set; }
    public EventCategoryType CategoryType { get; set; }
    public EventStatus Status { get; set; }
    public EventVisibility Visibility { get; set; }
    public bool AutoApprove { get; set; }
    public string? ImageUrl { get; set; }
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
