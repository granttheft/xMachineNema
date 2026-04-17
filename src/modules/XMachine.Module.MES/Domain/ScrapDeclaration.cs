using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class ScrapDeclaration : TenantAuditableEntity
{
    public Guid LotBatchId { get; set; }

    public decimal ScrapQuantity { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public DateTimeOffset DeclaredAt { get; set; } = DateTimeOffset.UtcNow;
    public string? Notes { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
