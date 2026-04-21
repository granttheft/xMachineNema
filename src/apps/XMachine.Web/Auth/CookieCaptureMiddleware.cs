using XMachine.Module.Auth.Security;

namespace XMachine.Web.Auth;

/// <summary>
/// Middleware that captures the auth cookie from each HTTP request
/// and stores it in the scoped <see cref="BlazorCookieStore"/>.
/// </summary>
public sealed class CookieCaptureMiddleware
{
    private readonly RequestDelegate _next;

    public CookieCaptureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, BlazorCookieStore store)
    {
        if (context.Request.Cookies.TryGetValue(
                XMachineAuthDefaults.CookieName, out var cookieValue)
            && !string.IsNullOrEmpty(cookieValue))
        {
            store.AuthCookieValue = cookieValue;
        }

        await _next(context).ConfigureAwait(false);
    }
}
