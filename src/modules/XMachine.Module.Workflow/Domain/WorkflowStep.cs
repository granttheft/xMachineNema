using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Workflow.Domain;

public sealed class WorkflowStep : TenantAuditableEntity
{
    public Guid WorkflowDefinitionId { get; set; }

    public int SequenceNo { get; set; }

    public string RoleCode { get; set; } = string.Empty;

    public WorkflowApprovalMode ApprovalMode { get; set; } = WorkflowApprovalMode.Sequential;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
