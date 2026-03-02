using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulse.Application.Features.Applications.Commands.ApplyToEvent;
using Pulse.Application.Features.Applications.Commands.ApproveApplication;
using Pulse.Application.Features.Applications.Commands.RejectApplication;
using Pulse.Application.Features.Applications.Commands.WithdrawApplication;
using Pulse.Application.Features.Applications.DTOs;
using Pulse.Application.Features.Applications.Queries.GetEventApplications;
using Pulse.Domain.Enums;

namespace Pulse.API.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/applications")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Apply(Guid eventId, [FromBody] ApplyToEventDto dto, CancellationToken cancellationToken)
    {
        var command = new ApplyToEventCommand
        {
            EventId = eventId,
            Message = dto.Message
        };

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetEventApplications), new { eventId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetEventApplications(
        Guid eventId,
        [FromQuery] ApplicationStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetEventApplicationsQuery
        {
            EventId = eventId,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{applicationId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid eventId, Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveApplicationCommand { ApplicationId = applicationId }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{applicationId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid eventId, Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectApplicationCommand { ApplicationId = applicationId }, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{applicationId:guid}")]
    public async Task<IActionResult> Withdraw(Guid eventId, Guid applicationId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new WithdrawApplicationCommand { ApplicationId = applicationId }, cancellationToken);
        return NoContent();
    }
}
