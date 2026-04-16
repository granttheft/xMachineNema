namespace XMachine.Connectors.Contracts;

public enum ConnectorConnectionState
{
    Unknown = 0,
    Disconnected = 1,
    Connecting = 2,
    Connected = 3,
    Degraded = 4,
    Faulted = 5,
}
