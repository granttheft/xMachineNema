using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Eventing.Domain;

/// <summary>Stored OEE calculation slice — no engine in this layer.</summary>
public sealed class OeeSnapshot : TenantAuditableEntity
{
    public Guid? SiteId { get; set; }
    public Guid? LineId { get; set; }
    public Guid? MachineId { get; set; }

    public OeePeriodType PeriodType { get; set; } = OeePeriodType.Shift;
    public DateTimeOffset PeriodStart { get; set; }
    public DateTimeOffset PeriodEnd { get; set; }

    public decimal Availability { get; set; }
    public decimal Performance { get; set; }
    public decimal Quality { get; set; }
    public decimal OeeValue { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
