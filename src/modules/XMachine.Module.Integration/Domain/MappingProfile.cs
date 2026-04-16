using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Integration.Domain;

public sealed class MappingProfile : TenantAuditableEntity
{
    public Guid ConnectorInstanceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
