using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Auth.Domain;

public sealed class UserAccount : TenantAuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

