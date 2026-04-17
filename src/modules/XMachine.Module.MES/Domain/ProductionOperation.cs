using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class ProductionOperation : TenantAuditableEntity
{
    public Guid ProductionOrderId { get; set; }

    public int SequenceNo { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Guid? LineId { get; set; }
    public Guid? MachineId { get; set; }

    public ProductionOperationStatus OperationStatus { get; set; } = ProductionOperationStatus.Pending;
    public decimal QuantityPlanned { get; set; }
    public decimal QuantityCompleted { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
