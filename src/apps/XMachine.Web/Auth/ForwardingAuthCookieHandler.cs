namespace XMachine.Web.Auth;

/// <summary>
/// Copies the browser cookie header to outbound API calls so the same auth ticket can be validated on XMachine.Api.
/// </summary>
public sealed class ForwardingAuthCookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForwardingAuthCookieHandler(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookie = _httpContextAccessor.HttpContext?.Request.Headers.Cookie.ToString();
        if (!string.IsNullOrWhiteSpace(cookie))
            request.Headers.TryAddWithoutValidation("Cookie", cookie);

        return base.SendAsync(request, cancellationToken);
    }
}
