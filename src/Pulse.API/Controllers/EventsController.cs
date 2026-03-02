using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulse.Application.Features.Events.Commands.CreateEvent;
using Pulse.Application.Features.Events.Commands.DeleteEvent;
using Pulse.Application.Features.Events.Commands.UpdateEvent;
using Pulse.Application.Features.Events.DTOs;
using Pulse.Application.Features.Events.Queries.GetEventById;
using Pulse.Application.Features.Events.Queries.GetHeatmapData;
using Pulse.Application.Features.Events.Queries.GetMyEvents;
using Pulse.Application.Features.Events.Queries.GetNearbyEvents;
using Pulse.Application.Features.Events.Queries.SearchEvents;
using Pulse.Domain.Enums;

namespace Pulse.API.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateEventCommand
        {
            Title = dto.Title,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Address = dto.Address,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            MaxCapacity = dto.MaxCapacity,
            CategoryType = dto.CategoryType,
            Visibility = dto.Visibility,
            AutoApprove = dto.AutoApprove,
            ImageUrl = dto.ImageUrl
        };

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEventByIdQuery { EventId = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateEventCommand
        {
            EventId = id,
            Title = dto.Title,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Address = dto.Address,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            MaxCapacity = dto.MaxCapacity,
            CategoryType = dto.CategoryType,
            Visibility = dto.Visibility,
            AutoApprove = dto.AutoApprove,
            ImageUrl = dto.ImageUrl
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteEventCommand { EventId = id }, cancellationToken);
        return NoContent();
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radius = 10,
        [FromQuery] EventCategoryType? category = null,
        [FromQuery] EventStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNearbyEventsQuery
        {
            Latitude = lat,
            Longitude = lng,
            RadiusKm = radius,
            Category = category,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("heatmap")]
    public async Task<IActionResult> GetHeatmap(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radius = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetHeatmapDataQuery
        {
            Latitude = lat,
            Longitude = lng,
            RadiusKm = radius
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyEventsQuery
        {
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q = null,
        [FromQuery] EventCategoryType? category = null,
        [FromQuery] EventStatus? status = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchEventsQuery
        {
            SearchTerm = q,
            Category = category,
            Status = status,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
