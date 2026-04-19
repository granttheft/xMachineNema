namespace XMachine.Module.Workflow.Domain;

public enum WorkflowInstanceState
{
    Draft = 1,
    InProgress = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5,
}
