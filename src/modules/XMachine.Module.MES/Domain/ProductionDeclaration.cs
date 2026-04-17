using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class ProductionDeclaration : TenantAuditableEntity
{
    public Guid LotBatchId { get; set; }

    public decimal GoodQuantity { get; set; }
    public DateTimeOffset DeclaredAt { get; set; } = DateTimeOffset.UtcNow;

    public Guid? LineId { get; set; }
    public Guid? MachineId { get; set; }
    public string? Notes { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
