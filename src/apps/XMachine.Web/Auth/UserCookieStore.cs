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

    public void Set(Guid userId, string cookieValue) =>
        _store[userId] = cookieValue;

    public string? Get(Guid userId) =>
        _store.TryGetValue(userId, out var value) ? value : null;

    public void Remove(Guid userId) =>
        _store.TryRemove(userId, out _);
}
