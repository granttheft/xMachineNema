using System.Security.Claims;
using XMachine.Module.Auth.Security;

namespace XMachine.Web.Auth;

/// <summary>
/// Captures auth cookie from each HTTP request and stores it
/// in <see cref="UserCookieStore"/> keyed by UserId (available in both HTTP
/// and Blazor circuit scopes).
/// </summary>
public sealed class CookieCaptureMiddleware
{
    private readonly RequestDelegate _next;

    public CookieCaptureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserCookieStore cookieStore)
    {
        if (context.Request.Cookies.TryGetValue(
                XMachineAuthDefaults.CookieName, out var cookieValue)
            && !string.IsNullOrEmpty(cookieValue)
            && context.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                cookieStore.Set(userId, cookieValue);
        }

        await _next(context).ConfigureAwait(false);
    }
}
