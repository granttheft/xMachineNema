using XMachine.Connectors.Abstractions;

namespace XMachine.Connectors.Runtime;

public sealed class PlaceholderConnectorFactory : IConnectorFactory
{
    public PlaceholderConnectorFactory(string connectorCode) => ConnectorCode = connectorCode;

    public string ConnectorCode { get; }

    public IConnector Create() => new PlaceholderConnector(ConnectorCode);
}
