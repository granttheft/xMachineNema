using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace XMachine.Module.Auth.Security;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var value = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var value = Principal?.FindFirst(XMachineClaims.TenantId)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Username =>
        Principal?.FindFirst("preferred_username")?.Value;

    public IReadOnlyList<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value).ToList()
        ?? new List<string>();
}
