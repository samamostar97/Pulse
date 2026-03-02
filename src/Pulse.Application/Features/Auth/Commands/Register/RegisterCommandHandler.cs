using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Pulse.Application.Common.Interfaces;
using Pulse.Application.Features.Auth.DTOs;
using Pulse.Domain.Entities;
using Pulse.Domain.Enums;

namespace Pulse.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLowerInvariant(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = UserRole.User,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
