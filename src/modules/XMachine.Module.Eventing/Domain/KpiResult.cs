using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Eventing.Domain;

public sealed class KpiResult : TenantAuditableEntity
{
    public Guid KpiDefinitionId { get; set; }

    public string ScopeType { get; set; } = "machine";
    public Guid ScopeId { get; set; }

    public DateTimeOffset PeriodStart { get; set; }
    public DateTimeOffset PeriodEnd { get; set; }

    public decimal Value { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
