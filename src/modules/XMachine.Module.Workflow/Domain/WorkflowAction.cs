using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Workflow.Domain;

public sealed class WorkflowAction : TenantAuditableEntity
{
    public Guid WorkflowInstanceId { get; set; }

    public Guid WorkflowStepId { get; set; }

    public WorkflowActionType ActionType { get; set; }

    public Guid? ActionBy { get; set; }

    public DateTimeOffset ActionTime { get; set; } = DateTimeOffset.UtcNow;

    public string? Comment { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
