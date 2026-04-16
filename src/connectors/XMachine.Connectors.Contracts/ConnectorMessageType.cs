namespace XMachine.Connectors.Contracts;

public enum ConnectorMessageType
{
    Telemetry = 0,
    Alarm = 1,
    StateChange = 2,
    ProductionEvent = 3,
    QualityEvent = 4,
    ErpOrderIn = 5,
    ErpResultOut = 6,
    CommandResult = 7,
}
