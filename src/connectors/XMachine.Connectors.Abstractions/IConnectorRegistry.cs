using System.Diagnostics.CodeAnalysis;

namespace XMachine.Connectors.Abstractions;

/// <summary>Registers connector factories by definition code (e.g. opcua, sap).</summary>
public interface IConnectorRegistry
{
    void Register(IConnectorFactory factory);

    bool TryGetFactory(string connectorCode, [NotNullWhen(true)] out IConnectorFactory? factory);

    IReadOnlyCollection<string> RegisteredCodes { get; }
}
