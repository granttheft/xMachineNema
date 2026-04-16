using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Abstractions;

public interface IConnector
{
    string ConnectorCode { get; }

    ValueTask ValidateConfigAsync(CancellationToken cancellationToken);
    ValueTask ConnectAsync(CancellationToken cancellationToken);
    ValueTask DisconnectAsync(CancellationToken cancellationToken);

    IAsyncEnumerable<ConnectorEnvelope> ReadSnapshotAsync(CancellationToken cancellationToken);
    IAsyncEnumerable<ConnectorEnvelope> StartStreamingAsync(CancellationToken cancellationToken);
    ValueTask StopStreamingAsync(CancellationToken cancellationToken);

    ValueTask<ConnectorHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken);
}
