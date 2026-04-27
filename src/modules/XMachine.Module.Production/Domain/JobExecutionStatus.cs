namespace XMachine.Module.Production.Domain;

public enum JobExecutionStatus
{
    Queued = 1,
    Setup = 2,
    Running = 3,
    Paused = 4,
    Done = 5,
    Cancelled = 6,
}
