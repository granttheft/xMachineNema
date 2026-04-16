using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Abstractions;

/// <summary>Optional write-back / command path (no concrete protocol here).</summary>
public interface ICommandCapableConnector : IConnector
{
    ValueTask<ConnectorResult> SendCommandAsync(ConnectorCommand command, CancellationToken cancellationToken);
}
