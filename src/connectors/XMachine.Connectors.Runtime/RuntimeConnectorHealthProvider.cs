using XMachine.Connectors.Abstractions;
using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Runtime;

public sealed class RuntimeConnectorHealthProvider : IConnectorHealthProvider
{
    private readonly ConnectorOrchestrator _orchestrator;

    public RuntimeConnectorHealthProvider(ConnectorOrchestrator orchestrator) =>
        _orchestrator = orchestrator;

    public ValueTask<IReadOnlyList<ConnectorHealthStatus>> GetSnapshotAsync(
        CancellationToken cancellationToken = default) =>
        new(_orchestrator.GetHealthSnapshotAsync(cancellationToken));
}
