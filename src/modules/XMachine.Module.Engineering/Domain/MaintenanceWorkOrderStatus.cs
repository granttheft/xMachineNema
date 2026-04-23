namespace XMachine.Module.Engineering.Domain;

public enum MaintenanceWorkOrderStatus
{
    Open = 1,
    InProgress = 2,
    WaitingParts = 3,
    Done = 4,
    Cancelled = 5,
}
