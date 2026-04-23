using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Engineering.Domain;

public sealed class MachineFault : TenantAuditableEntity
{
    public Guid MachineId { get; set; }
    public Guid? MaintenanceWorkOrderId { get; set; }

    public string FaultCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid ReportedBy { get; set; }
    public DateTimeOffset ReportedAt { get; set; }

    public bool Resolved { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
