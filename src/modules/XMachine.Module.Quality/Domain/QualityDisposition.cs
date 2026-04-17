using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Quality.Domain;

public sealed class QualityDisposition : TenantAuditableEntity
{
    public Guid NonconformanceId { get; set; }

    public QualityDispositionType DispositionType { get; set; } = QualityDispositionType.Other;
    public DateTimeOffset DecisionTime { get; set; } = DateTimeOffset.UtcNow;
    public Guid? DecidedBy { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
