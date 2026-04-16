namespace XMachine.Connectors.Contracts;

/// <summary>Placeholder for connector command / sync outcomes.</summary>
public sealed record ConnectorResult(
    bool Success,
    string? Message,
    string? DetailJson
);
