namespace XMachine.Connectors.Contracts;

/// <summary>Placeholder for controlled write-back / command pipeline.</summary>
public sealed record ConnectorCommand(
    Guid TenantId,
    string ConnectorCode,
    string CommandType,
    IReadOnlyDictionary<string, string>? Parameters
);
