namespace XMachine.Module.Production.Domain;

public enum JobPauseReason
{
    MaterialShortage = 1,
    QualityIssue = 2,
    MachineBreakdown = 3,
    OperatorBreak = 4,
    PlannedStop = 5,
    WaitingSetup = 6,
    Other = 7,
}
