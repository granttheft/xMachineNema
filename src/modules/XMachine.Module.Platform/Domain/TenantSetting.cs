using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Platform.Domain;

public sealed class TenantSetting : TenantAuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

