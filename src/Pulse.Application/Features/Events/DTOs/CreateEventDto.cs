using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Events.DTOs;

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int MaxCapacity { get; set; }
    public EventCategoryType CategoryType { get; set; }
    public EventVisibility Visibility { get; set; }
    public bool AutoApprove { get; set; }
    public string? ImageUrl { get; set; }
}
