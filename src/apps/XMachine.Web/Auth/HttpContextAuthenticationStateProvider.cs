using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace XMachine.Web.Auth;

/// <summary>Surfaces <see cref="Microsoft.AspNetCore.Http.HttpContext.User"/> into Blazor.</summary>
public sealed class HttpContextAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User
                   ?? new ClaimsPrincipal(new ClaimsIdentity());
        return Task.FromResult(new AuthenticationState(user));
    }
}
