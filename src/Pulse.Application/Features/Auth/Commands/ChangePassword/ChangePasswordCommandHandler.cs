using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Pulse.Application.Common.Interfaces;
using Pulse.Domain.Entities;

namespace Pulse.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ChangePasswordCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
