using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Commercial.Domain;

public sealed class LicensedLine : TenantAuditableEntity
{
    public Guid LineId { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

