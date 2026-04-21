namespace XMachine.Web.Auth;

/// <summary>
/// Scoped service that captures the auth cookie value during the initial
/// HTTP request and makes it available throughout the Blazor Server circuit.
/// </summary>
public sealed class BlazorCookieStore
{
    /// <summary>Raw cookie value (without the name prefix).</summary>
    public string? AuthCookieValue { get; set; }
}
