using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Eventing.Domain;

public sealed class AlarmEvent : TenantAuditableEntity
{
    public Guid? SiteId { get; set; }
    public Guid? LineId { get; set; }
    public Guid? MachineId { get; set; }

    public string AlarmCode { get; set; } = string.Empty;
    public string AlarmText { get; set; } = string.Empty;
    public AlarmSeverity Severity { get; set; } = AlarmSeverity.Warning;
    public string Category { get; set; } = string.Empty;

    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndTime { get; set; }
    public long? DurationMs { get; set; }

    public Guid? AckBy { get; set; }
    public DateTimeOffset? AckTime { get; set; }

    public AlarmLifecycleStatus AlarmStatus { get; set; } = AlarmLifecycleStatus.Active;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
