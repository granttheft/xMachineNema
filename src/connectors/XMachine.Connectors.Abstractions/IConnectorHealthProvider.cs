using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Abstractions;

/// <summary>Aggregate health view over runtime connectors (placeholder-friendly).</summary>
public interface IConnectorHealthProvider
{
    ValueTask<IReadOnlyList<ConnectorHealthStatus>> GetSnapshotAsync(CancellationToken cancellationToken = default);
}
