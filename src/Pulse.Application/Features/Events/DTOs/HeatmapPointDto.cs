namespace Pulse.Application.Features.Events.DTOs;

public class HeatmapPointDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Weight { get; set; }
    public Guid EventId { get; set; }
}
