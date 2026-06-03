using System.Security.Claims;
using Enrollment.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Enrollment.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            string? sub = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

            if (sub is null || !Guid.TryParse(sub, out Guid userId))
                throw new UnauthorizedAccessException("User identity not found in token.");

            return userId;
        }
    }
}
