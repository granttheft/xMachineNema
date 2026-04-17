using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Quality.Domain;

public sealed class QualityCheck : TenantAuditableEntity
{
    public Guid? ProductionOrderId { get; set; }
    public Guid? LotBatchId { get; set; }
    public Guid? MachineId { get; set; }

    public string CheckType { get; set; } = string.Empty;
    public DateTimeOffset CheckTime { get; set; } = DateTimeOffset.UtcNow;

    public QualityCheckStatus CheckStatus { get; set; } = QualityCheckStatus.Pending;
    public Guid? ApprovedBy { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
