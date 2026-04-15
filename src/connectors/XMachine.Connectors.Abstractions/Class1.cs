using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Abstractions;

public interface IConnector
{
    string ConnectorCode { get; }

    ValueTask ValidateConfigAsync(CancellationToken cancellationToken);
    ValueTask ConnectAsync(CancellationToken cancellationToken);
    ValueTask DisconnectAsync(CancellationToken cancellationToken);

    IAsyncEnumerable<CanonicalMessageEnvelope> ReadSnapshotAsync(CancellationToken cancellationToken);
    IAsyncEnumerable<CanonicalMessageEnvelope> StartStreamingAsync(CancellationToken cancellationToken);
    ValueTask StopStreamingAsync(CancellationToken cancellationToken);

    ValueTask<ConnectorHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken);
}

public interface IConnectorFactory
{
    string ConnectorCode { get; }
    IConnector Create();
}
