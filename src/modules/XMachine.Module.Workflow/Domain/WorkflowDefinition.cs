using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Workflow.Domain;

public sealed class WorkflowDefinition : TenantAuditableEntity
{
    public string WorkflowType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
