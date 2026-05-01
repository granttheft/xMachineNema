namespace XMachine.Api.Hubs;

/// <summary>
/// Strongly typed client contract for server-to-client monitoring pushes.
/// </summary>
public interface IXMachineHubClient
{
    /// <summary>Notifies clients that a machine operational status changed.</summary>
    Task MachineStatusChanged(MachineStatusChangedEvent evt);

    /// <summary>Notifies clients that a job execution status or counters changed.</summary>
    Task JobStatusChanged(JobStatusChangedEvent evt);

    /// <summary>Notifies clients that a new alarm was raised.</summary>
    Task AlarmTriggered(AlarmTriggeredEvent evt);

    /// <summary>Notifies clients that OEE metrics were updated for a machine.</summary>
    Task OeeUpdated(OeeUpdatedEvent evt);
}

/// <summary>Payload for machine operational status changes.</summary>
public sealed record MachineStatusChangedEvent(
    Guid MachineId,
    string MachineCode,
    string MachineName,
    string OldStatus,
    string NewStatus,
    DateTimeOffset ChangedAt);

/// <summary>Payload for job execution status or quantity updates.</summary>
public sealed record JobStatusChangedEvent(
    Guid JobId,
    string JobNo,
    Guid MachineId,
    string OldStatus,
    string NewStatus,
    DateTimeOffset ChangedAt,
    int ProducedQty,
    int PlannedQty);

/// <summary>Payload when a new alarm is triggered.</summary>
public sealed record AlarmTriggeredEvent(
    Guid AlarmId,
    Guid MachineId,
    string AlarmCode,
    string Message,
    string Severity,
    DateTimeOffset TriggeredAt);

/// <summary>Payload when OEE snapshot values change for a machine.</summary>
public sealed record OeeUpdatedEvent(
    Guid MachineId,
    decimal Oee,
    decimal Availability,
    decimal Performance,
    decimal Quality,
    DateTimeOffset SnapshotAt);
