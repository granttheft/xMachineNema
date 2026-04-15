using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Platform.Domain;

public sealed class Station : TenantAuditableEntity
{
    public Guid MachineId { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

