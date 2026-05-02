namespace XMachine.Module.Planning.Domain;

/// <summary>Execution state of a scheduled <see cref="PlanSlot"/>.</summary>
public enum PlanSlotStatus
{
    Scheduled = 1,
    Running = 2,
    Done = 3,
    Skipped = 4,
    Cancelled = 5,
}
