using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class ProductionOrder : TenantAuditableEntity
{
    public Guid? SiteId { get; set; }
    public Guid? LineId { get; set; }

    public string OrderNo { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal QuantityPlanned { get; set; }
    public decimal QuantityCompleted { get; set; }

    public ProductionOrderStatus OrderStatus { get; set; } = ProductionOrderStatus.Draft;

    public DateTimeOffset? PlannedStartAt { get; set; }
    public DateTimeOffset? PlannedEndAt { get; set; }
    public DateTimeOffset? ActualStartAt { get; set; }
    public DateTimeOffset? ActualEndAt { get; set; }

    public string? SourceSystem { get; set; }
    public string? SourceReference { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
