using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Auth.DTOs;

namespace Pulse.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _jwtTokenService.ValidateAccessToken(request.AccessToken);

        if (userId == null)
            throw new UnauthorizedAccessException("Invalid access token.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Your account has been suspended.");

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
