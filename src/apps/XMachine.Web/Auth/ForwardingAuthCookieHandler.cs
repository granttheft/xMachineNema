using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using XMachine.Module.Auth.Security;

namespace XMachine.Web.Auth;

/// <summary>
/// Forwards auth cookie to API using 3 strategies:
/// 1. Direct HttpContext read (SSR phase)
/// 2. UserId lookup in store (circuit phase, when auth state available)
/// 3. Most recent stored cookie (circuit fallback)
/// </summary>
public sealed class ForwardingAuthCookieHandler : DelegatingHandler
{
    private readonly UserCookieStore _cookieStore;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationStateProvider _authStateProvider;

    public ForwardingAuthCookieHandler(
        UserCookieStore cookieStore,
        IHttpContextAccessor httpContextAccessor,
        AuthenticationStateProvider authStateProvider)
    {
        _cookieStore = cookieStore;
        _httpContextAccessor = httpContextAccessor;
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var cookieValue = await GetCookieValueAsync().ConfigureAwait(false);

        if (!string.IsNullOrEmpty(cookieValue))
        {
            request.Headers.TryAddWithoutValidation(
                "Cookie",
                $"{XMachineAuthDefaults.CookieName}={cookieValue}");
        }

        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<string?> GetCookieValueAsync()
    {
        // Strategy 1: Read directly from current HTTP request (works in SSR phase)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null &&
            httpContext.Request.Cookies.TryGetValue(
                XMachineAuthDefaults.CookieName, out var directCookie) &&
            !string.IsNullOrEmpty(directCookie))
        {
            return directCookie;
        }

        // Strategy 2: Auth state → userId → store lookup (circuit phase)
        try
        {
            var authState = await _authStateProvider
                .GetAuthenticationStateAsync()
                .ConfigureAwait(false);

            var userIdValue = authState.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdValue, out var userId))
            {
                var stored = _cookieStore.Get(userId);
                if (!string.IsNullOrEmpty(stored))
                    return stored;
            }
        }
        catch
        {
            // Never block the request
        }

        // Strategy 3: Most recently stored cookie (circuit fallback)
        return _cookieStore.GetMostRecent();
    }
}
