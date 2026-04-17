using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class Shift : TenantAuditableEntity
{
    public Guid SiteId { get; set; }
    public Guid? LineId { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public DateOnly ShiftDate { get; set; }

    public DateTimeOffset PlannedStartAt { get; set; }
    public DateTimeOffset PlannedEndAt { get; set; }
    public DateTimeOffset? ActualStartAt { get; set; }
    public DateTimeOffset? ActualEndAt { get; set; }

    public WorkShiftLifecycle Lifecycle { get; set; } = WorkShiftLifecycle.Planned;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
