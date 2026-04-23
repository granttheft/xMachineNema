namespace XMachine.Module.Platform.Domain;

public enum MachineOperationalStatus
{
    Running = 1,
    Idle = 2,
    Down = 3,
    PmDue = 4,
    WaitingParts = 5,
}
