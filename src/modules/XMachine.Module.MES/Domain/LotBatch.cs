using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class LotBatch : TenantAuditableEntity
{
    public Guid ProductionOrderId { get; set; }

    public Guid? LineId { get; set; }
    public Guid? MachineId { get; set; }

    public string LotNo { get; set; } = string.Empty;
    public LotBatchStatus LotStatus { get; set; } = LotBatchStatus.Planned;

    public decimal? TargetQuantity { get; set; }
    public decimal QuantityGood { get; set; }

    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
