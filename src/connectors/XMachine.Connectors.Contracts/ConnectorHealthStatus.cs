namespace XMachine.Connectors.Contracts;

public sealed record ConnectorHealthStatus(
    string ConnectorCode,
    ConnectorConnectionState ConnectionState,
    DateTimeOffset? LastHeartbeatAt,
    DateTimeOffset? LastSuccessfulReadAt,
    DateTimeOffset? LastSuccessfulWriteAt,
    long ErrorCount,
    string? LastError
);
