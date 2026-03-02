using MediatR;
using Pulse.Application.Features.Events.DTOs;

namespace Pulse.Application.Features.Events.Queries.GetHeatmapData;

public class GetHeatmapDataQuery : IRequest<List<HeatmapPointDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10;
}
