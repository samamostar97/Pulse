using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Pulse.Application.Common.Interfaces;

namespace Pulse.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return userId != null ? Guid.Parse(userId) : null;
        }
    }

    public string? Role =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
