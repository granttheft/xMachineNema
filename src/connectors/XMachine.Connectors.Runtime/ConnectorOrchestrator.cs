using Microsoft.Extensions.Logging;
using XMachine.Connectors.Abstractions;
using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Runtime;

/// <summary>
/// Minimal lifecycle coordinator: discovers factories via <see cref="IConnectorRegistry"/>,
/// does not read operational DB instance rows (that stays in API / future resolver).
/// </summary>
public sealed class ConnectorOrchestrator
{
    private readonly IConnectorRegistry _registry;
    private readonly ILogger<ConnectorOrchestrator> _logger;
    private readonly Dictionary<string, IConnector> _running = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _lifecycleLock = new(1, 1);

    public ConnectorOrchestrator(IConnectorRegistry registry, ILogger<ConnectorOrchestrator> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    public IReadOnlyCollection<string> ActiveConnectorCodes
    {
        get
        {
            lock (_running)
                return _running.Keys.ToArray();
        }
    }

    public async Task ValidateRegisteredAsync(CancellationToken cancellationToken = default)
    {
        foreach (var code in _registry.RegisteredCodes)
        {
            if (!_registry.TryGetFactory(code, out var factory) || factory is null)
                continue;

            var connector = factory.Create();
            await connector.ValidateConfigAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogDebug("Validated placeholder connector {Code}", code);
        }
    }

    public async Task StartRegisteredAsync(CancellationToken cancellationToken = default)
    {
        await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            foreach (var code in _registry.RegisteredCodes)
            {
                if (!_registry.TryGetFactory(code, out var factory) || factory is null)
                    continue;

                var connector = factory.Create();
                await connector.ValidateConfigAsync(cancellationToken).ConfigureAwait(false);
                await connector.ConnectAsync(cancellationToken).ConfigureAwait(false);

                lock (_running)
                    _running[code] = connector;

                _logger.LogInformation("Started placeholder connector {Code}", code);
            }
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    public async Task StopRegisteredAsync(CancellationToken cancellationToken = default)
    {
        await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            List<IConnector> snapshot;
            lock (_running)
            {
                snapshot = _running.Values.ToList();
                _running.Clear();
            }

            foreach (var connector in snapshot)
            {
                await connector.StopStreamingAsync(cancellationToken).ConfigureAwait(false);
                await connector.DisconnectAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Stopped placeholder connector {Code}", connector.ConnectorCode);
            }
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    public async Task<IReadOnlyList<ConnectorHealthStatus>> GetHealthSnapshotAsync(
        CancellationToken cancellationToken = default)
    {
        List<IConnector> snapshot;
        lock (_running)
            snapshot = _running.Values.ToList();

        if (snapshot.Count == 0)
            return Array.Empty<ConnectorHealthStatus>();

        var list = new List<ConnectorHealthStatus>(snapshot.Count);
        foreach (var connector in snapshot)
        {
            var h = await connector.GetHealthStatusAsync(cancellationToken).ConfigureAwait(false);
            list.Add(h);
        }

        return list;
    }
}
