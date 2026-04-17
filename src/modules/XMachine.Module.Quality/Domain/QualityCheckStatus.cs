namespace XMachine.Module.Quality.Domain;

public enum QualityCheckStatus
{
    Pending = 1,
    InProgress = 2,
    Passed = 3,
    Failed = 4,
    Waived = 5,
}
