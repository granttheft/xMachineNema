namespace XMachine.Module.Planning.Domain;

/// <summary>Lifecycle state of a <see cref="ProductionPlan"/>.</summary>
public enum PlanStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    InProgress = 4,
    Done = 5,
    Cancelled = 6,
}
