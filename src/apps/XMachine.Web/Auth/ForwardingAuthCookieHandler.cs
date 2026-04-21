using Microsoft.AspNetCore.Http;
using XMachine.Module.Auth.Security;

namespace XMachine.Web.Auth;

/// <summary>
/// Forwards the xMachine auth cookie from the incoming browser request
/// to outgoing API HttpClient requests.
/// </summary>
public sealed class ForwardingAuthCookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForwardingAuthCookieHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            // Forward the auth cookie
            if (httpContext.Request.Cookies.TryGetValue(
                XMachineAuthDefaults.CookieName, out var cookieValue)
                && !string.IsNullOrEmpty(cookieValue))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    $"{XMachineAuthDefaults.CookieName}={cookieValue}");
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
