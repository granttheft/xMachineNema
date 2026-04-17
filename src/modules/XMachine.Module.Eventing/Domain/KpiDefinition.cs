using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Eventing.Domain;

public sealed class KpiDefinition : TenantAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? FormulaExpression { get; set; }

    public string ScopeType { get; set; } = "machine";
    public string DataSourceType { get; set; } = "manual";

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
