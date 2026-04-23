using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Engineering.Domain;

public sealed class MaintenanceWorkOrder : TenantAuditableEntity
{
    public Guid MachineId { get; set; }
    public Guid? LineId { get; set; }

    public string WorkOrderNo { get; set; } = string.Empty;
    public MaintenanceWorkOrderType WorkOrderType { get; set; }
    public MaintenancePriority Priority { get; set; }
    public MaintenanceWorkOrderStatus WorkOrderStatus { get; set; }

    public string Description { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }

    public Guid? AssignedTo { get; set; }
    public string? AssignedRole { get; set; }

    public DateTimeOffset ReportedAt { get; set; }
    public Guid ReportedBy { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? ClosingNotes { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
