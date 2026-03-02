using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulse.Application.Features.Auth.Commands.ChangePassword;
using Pulse.Application.Features.Auth.Commands.Login;
using Pulse.Application.Features.Auth.Commands.Register;
using Pulse.Application.Features.Auth.Commands.RefreshToken;
using Pulse.Application.Features.Auth.DTOs;

namespace Pulse.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand
        {
            Email = dto.Email,
            Password = dto.Password,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var command = new LoginCommand
        {
            Email = dto.Email,
            Password = dto.Password
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand
        {
            CurrentPassword = dto.CurrentPassword,
            NewPassword = dto.NewPassword
        };

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
