using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using XMachine.Module.Auth.Security;

namespace XMachine.Web.Auth;

/// <summary>
/// Forwards auth cookie to API calls.
/// Uses <see cref="AuthenticationStateProvider"/> (works in Blazor Server circuits)
/// to get UserId, then looks up cookie from <see cref="UserCookieStore"/>.
/// </summary>
public sealed class ForwardingAuthCookieHandler : DelegatingHandler
{
    private readonly UserCookieStore _cookieStore;
    private readonly AuthenticationStateProvider _authStateProvider;

    public ForwardingAuthCookieHandler(
        UserCookieStore cookieStore,
        AuthenticationStateProvider authStateProvider)
    {
        _cookieStore = cookieStore;
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var authState = await _authStateProvider
                .GetAuthenticationStateAsync()
                .ConfigureAwait(false);

            var userIdClaim = authState.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var cookieValue = _cookieStore.Get(userId);
                if (!string.IsNullOrEmpty(cookieValue))
                {
                    request.Headers.TryAddWithoutValidation(
                        "Cookie",
                        $"{XMachineAuthDefaults.CookieName}={cookieValue}");
                }
            }
        }
        catch
        {
            // Never block the request if auth lookup fails
        }

        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
