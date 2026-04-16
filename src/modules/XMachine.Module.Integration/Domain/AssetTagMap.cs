using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Integration.Domain;

public sealed class AssetTagMap : TenantAuditableEntity
{
    public Guid ConnectorInstanceId { get; set; }
    public Guid? MachineId { get; set; }
    public string SourceAddress { get; set; } = string.Empty;
    public string CanonicalField { get; set; } = string.Empty;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
