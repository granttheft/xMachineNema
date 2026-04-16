using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using XMachine.Connectors.Abstractions;

namespace XMachine.Connectors.Runtime;

public sealed class ConnectorRegistry : IConnectorRegistry
{
    private readonly ConcurrentDictionary<string, IConnectorFactory> _factories = new(StringComparer.OrdinalIgnoreCase);

    public void Register(IConnectorFactory factory) =>
        _factories[factory.ConnectorCode] = factory;

    public bool TryGetFactory(string connectorCode, [NotNullWhen(true)] out IConnectorFactory? factory) =>
        _factories.TryGetValue(connectorCode, out factory);

    public IReadOnlyCollection<string> RegisteredCodes => _factories.Keys.ToArray();
}
