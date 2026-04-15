using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Commercial.Domain;

public sealed class TenantModuleActivation : TenantAuditableEntity
{
    public Guid ModuleId { get; set; }
    public DateTimeOffset ActivatedAt { get; set; } = DateTimeOffset.UtcNow;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

