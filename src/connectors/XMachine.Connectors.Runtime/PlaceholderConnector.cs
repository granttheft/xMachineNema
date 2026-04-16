using System.Runtime.CompilerServices;
using XMachine.Connectors.Abstractions;
using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Runtime;

/// <summary>No-protocol placeholder for host wiring and lifecycle smoke tests.</summary>
public sealed class PlaceholderConnector : IConnector
{
    public PlaceholderConnector(string connectorCode) => ConnectorCode = connectorCode;

    public string ConnectorCode { get; }

    public ValueTask ValidateConfigAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public async IAsyncEnumerable<ConnectorEnvelope> ReadSnapshotAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        yield break;
    }

    public async IAsyncEnumerable<ConnectorEnvelope> StartStreamingAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        yield break;
    }

    public ValueTask StopStreamingAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask<ConnectorHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken) =>
        ValueTask.FromResult(
            new ConnectorHealthStatus(
                ConnectorCode,
                ConnectorConnectionState.Connected,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                null,
                0,
                null));
}
