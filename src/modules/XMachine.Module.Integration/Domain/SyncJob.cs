using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Integration.Domain;

public sealed class SyncJob : TenantAuditableEntity
{
    public Guid ConnectorInstanceId { get; set; }
    public string JobType { get; set; } = "poll";
    public SyncJobStatus JobStatus { get; set; } = SyncJobStatus.Pending;
    public string? PayloadJson { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
