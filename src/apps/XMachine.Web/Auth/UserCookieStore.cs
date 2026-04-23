using System.Collections.Concurrent;

namespace XMachine.Web.Auth;

/// <summary>
/// Singleton store that maps UserId → auth cookie value.
/// Bridges the gap between HTTP request scope (middleware)
/// and Blazor Server circuit scope (components).
/// </summary>
public sealed class UserCookieStore
{
    private readonly ConcurrentDictionary<Guid, string> _store = new();
    private volatile string? _mostRecentCookieValue;

    /// <summary>
    /// Stores the cookie value for the given user and updates the most recent cookie cache.
    /// </summary>
    public void Set(Guid userId, string cookieValue)
    {
        _store[userId] = cookieValue;
        _mostRecentCookieValue = cookieValue;
    }

    /// <summary>
    /// Gets the cookie value for the given user if present.
    /// </summary>
    public string? Get(Guid userId) =>
        _store.TryGetValue(userId, out var value) ? value : null;

    /// <summary>
    /// Gets the most recently stored cookie value (fallback for circuit phase).
    /// </summary>
    public string? GetMostRecent() => _mostRecentCookieValue;

    /// <summary>
    /// Removes the cookie value for the given user.
    /// </summary>
    public void Remove(Guid userId) =>
        _store.TryRemove(userId, out _);
}
