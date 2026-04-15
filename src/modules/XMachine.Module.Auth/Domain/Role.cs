using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Auth.Domain;

public sealed class Role : TenantAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

