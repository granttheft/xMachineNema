namespace XMachine.Connectors.Contracts;

/// <summary>Standard inbound/outbound message wrapper (docs/05 envelope).</summary>
public sealed record ConnectorEnvelope(
    Guid TenantId,
    Guid? SiteId,
    Guid? LineId,
    Guid? MachineId,
    Guid? StationId,
    string ConnectorCode,
    ConnectorMessageType MessageType,
    DateTimeOffset Timestamp,
    IReadOnlyDictionary<string, CanonicalValue>? CanonicalFields,
    string? SourceReference = null,
    string? Quality = null
);
