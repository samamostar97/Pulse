using Pulse.Domain.Enums;
using Pulse.Domain.ValueObjects;

namespace Pulse.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Location Location { get; set; } = new();
    public string? Address { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int MaxCapacity { get; set; }
    public EventCategoryType CategoryType { get; set; }
    public EventStatus Status { get; set; }
    public EventVisibility Visibility { get; set; }
    public bool AutoApprove { get; set; }
    public string? ImageUrl { get; set; }
    public Guid OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<EventApplication> Applications { get; set; } = new List<EventApplication>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
