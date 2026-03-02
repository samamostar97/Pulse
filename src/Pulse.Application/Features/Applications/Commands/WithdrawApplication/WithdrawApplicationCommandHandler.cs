using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Application.Common.Interfaces;

namespace Pulse.Application.Features.Applications.Commands.WithdrawApplication;

public class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public WithdrawApplicationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var application = await _context.EventApplications
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (application.ApplicantId != userId)
            throw new UnauthorizedAccessException("You can only withdraw your own application.");

        _context.EventApplications.Remove(application);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
