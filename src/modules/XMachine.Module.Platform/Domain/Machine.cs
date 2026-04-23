using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Platform.Domain;

public sealed class Machine : TenantAuditableEntity
{
    public Guid LineId { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EntityStatus Status { get; set; } = EntityStatus.Active;

    public MachineOperationalStatus OperationalStatus { get; set; } = MachineOperationalStatus.Idle;
    public string? MachineType { get; set; }
    public string? Location { get; set; }
}

