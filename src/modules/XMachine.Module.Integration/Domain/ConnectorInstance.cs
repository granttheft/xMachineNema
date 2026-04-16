using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Integration.Domain;

public sealed class ConnectorInstance : TenantAuditableEntity
{
    public Guid ConnectorDefinitionId { get; set; }
    public Guid? SiteId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ConfigurationJson { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
