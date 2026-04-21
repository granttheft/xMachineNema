using XMachine.Module.Auth.Security;

namespace XMachine.Web.Auth;

/// <summary>
/// Forwards the captured auth cookie to outgoing API HttpClient requests.
/// Uses <see cref="BlazorCookieStore"/> instead of HTTP context
/// because <c>HttpContext</c> can be null during Blazor Server SignalR rendering.
/// </summary>
public sealed class ForwardingAuthCookieHandler : DelegatingHandler
{
    private readonly BlazorCookieStore _cookieStore;

    public ForwardingAuthCookieHandler(BlazorCookieStore cookieStore)
    {
        _cookieStore = cookieStore;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_cookieStore.AuthCookieValue))
        {
            request.Headers.TryAddWithoutValidation(
                "Cookie",
                $"{XMachineAuthDefaults.CookieName}={_cookieStore.AuthCookieValue}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
