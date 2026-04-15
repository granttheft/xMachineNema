using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Commercial.Domain;

public sealed class PackageCatalog : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

