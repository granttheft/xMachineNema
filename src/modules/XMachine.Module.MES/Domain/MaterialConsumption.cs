using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class MaterialConsumption : TenantAuditableEntity
{
    public Guid LotBatchId { get; set; }

    public string MaterialCode { get; set; } = string.Empty;
    public string? MaterialName { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "ea";
    public DateTimeOffset ConsumedAt { get; set; } = DateTimeOffset.UtcNow;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
