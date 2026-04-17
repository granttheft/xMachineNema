using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Eventing.Domain;

public sealed class DowntimeRecord : TenantAuditableEntity
{
    public Guid? MachineId { get; set; }
    public Guid? LineId { get; set; }
    public Guid? ProductionOrderId { get; set; }

    public string DowntimeReasonCode { get; set; } = string.Empty;
    public string? DowntimeReasonText { get; set; }
    public bool PlannedFlag { get; set; }

    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndTime { get; set; }
    public long? DurationMs { get; set; }

    public Guid? EnteredBy { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
