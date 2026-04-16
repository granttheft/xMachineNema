namespace XMachine.Connectors.Abstractions;

public interface IConnectorFactory
{
    string ConnectorCode { get; }
    IConnector Create();
}
