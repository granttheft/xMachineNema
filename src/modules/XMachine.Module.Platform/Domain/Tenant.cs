using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Platform.Domain;

public sealed class Tenant : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

