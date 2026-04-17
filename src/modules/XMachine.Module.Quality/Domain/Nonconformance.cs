using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Quality.Domain;

public sealed class Nonconformance : TenantAuditableEntity
{
    public Guid QualityCheckId { get; set; }

    public string NcCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public NonconformanceSeverity Severity { get; set; } = NonconformanceSeverity.Minor;
    public NonconformanceStatus NcStatus { get; set; } = NonconformanceStatus.Open;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
