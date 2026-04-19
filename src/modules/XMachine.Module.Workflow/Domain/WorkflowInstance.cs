using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Workflow.Domain;

public sealed class WorkflowInstance : TenantAuditableEntity
{
    public Guid WorkflowDefinitionId { get; set; }

    public string ReferenceType { get; set; } = string.Empty;

    public Guid ReferenceId { get; set; }

    public WorkflowInstanceState WorkflowState { get; set; } = WorkflowInstanceState.Draft;

    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? EndedAt { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
