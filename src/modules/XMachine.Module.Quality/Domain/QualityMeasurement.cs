using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Quality.Domain;

public sealed class QualityMeasurement : TenantAuditableEntity
{
    public Guid QualityCheckId { get; set; }

    public string ParameterCode { get; set; } = string.Empty;
    public string? MeasuredValue { get; set; }
    public string? TargetValue { get; set; }
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }

    public QualityMeasurementResult Result { get; set; } = QualityMeasurementResult.NotEvaluated;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
