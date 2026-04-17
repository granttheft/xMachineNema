using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

/// <summary>Placeholder inventory trace row; detailed WMS logic comes later.</summary>
public sealed class InventoryMovement : TenantAuditableEntity
{
    public InventoryMovementType MovementType { get; set; }

    public string MaterialCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "ea";

    public Guid? LotBatchId { get; set; }
    public Guid? ProductionOrderId { get; set; }

    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }

    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public string? Note { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
