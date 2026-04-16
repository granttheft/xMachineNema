using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Integration.Domain;

/// <summary>Product connector type (OPC UA, S7, …). Metadata only — no protocol logic.</summary>
public sealed class ConnectorDefinition : TenantAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ConnectorDirection Direction { get; set; } = ConnectorDirection.Inbound;
    public bool SupportsRead { get; set; } = true;
    public bool SupportsWrite { get; set; }
    public string? CapabilitiesJson { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
