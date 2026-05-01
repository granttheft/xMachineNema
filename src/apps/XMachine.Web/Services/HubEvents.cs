namespace XMachine.Web.Services;

/// <summary>Machine operational status change (matches API hub contract).</summary>
public sealed record MachineStatusChangedEvent(
    Guid MachineId,
    string MachineCode,
    string MachineName,
    string OldStatus,
    string NewStatus,
    DateTimeOffset ChangedAt);

/// <summary>Job execution status or quantity update (matches API hub contract).</summary>
public sealed record JobStatusChangedEvent(
    Guid JobId,
    string JobNo,
    Guid MachineId,
    string OldStatus,
    string NewStatus,
    DateTimeOffset ChangedAt,
    int ProducedQty,
    int PlannedQty);

/// <summary>New alarm raised (matches API hub contract).</summary>
public sealed record AlarmTriggeredEvent(
    Guid AlarmId,
    Guid MachineId,
    string AlarmCode,
    string Message,
    string Severity,
    DateTimeOffset TriggeredAt);

/// <summary>OEE snapshot update (matches API hub contract).</summary>
public sealed record OeeUpdatedEvent(
    Guid MachineId,
    decimal Oee,
    decimal Availability,
    decimal Performance,
    decimal Quality,
    DateTimeOffset SnapshotAt);
