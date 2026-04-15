namespace XMachine.Connectors.Contracts;

public enum ConnectorMessageType
{
    Telemetry = 0,
    Alarm = 1,
    StateChange = 2,
    ProductionEvent = 3,
    QualityEvent = 4,
    ErpOrderIn = 5,
    ErpResultOut = 6,
    CommandResult = 7,
}

public sealed record CanonicalMessageEnvelope(
    Guid TenantId,
    Guid? SiteId,
    Guid? LineId,
    Guid? MachineId,
    Guid? StationId,
    string ConnectorCode,
    ConnectorMessageType MessageType,
    DateTimeOffset Timestamp,
    object Payload,
    string? SourceReference = null,
    string? Quality = null
);

public enum ConnectorConnectionState
{
    Unknown = 0,
    Disconnected = 1,
    Connecting = 2,
    Connected = 3,
    Degraded = 4,
    Faulted = 5,
}

public sealed record ConnectorHealthStatus(
    string ConnectorCode,
    ConnectorConnectionState ConnectionState,
    DateTimeOffset? LastHeartbeatAt,
    DateTimeOffset? LastSuccessfulReadAt,
    DateTimeOffset? LastSuccessfulWriteAt,
    long ErrorCount,
    string? LastError
);
