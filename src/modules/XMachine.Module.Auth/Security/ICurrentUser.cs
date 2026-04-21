namespace XMachine.Module.Auth.Security;

public interface ICurrentUser
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
}
