using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Engineering.Domain;

public sealed class PreventiveMaintenanceSchedule : TenantAuditableEntity
{
    public Guid MachineId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int? IntervalHours { get; set; }
    public int? IntervalDays { get; set; }

    public DateTimeOffset? LastDoneAt { get; set; }
    public DateTimeOffset? NextDueAt { get; set; }

    public string? OwnerRole { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
