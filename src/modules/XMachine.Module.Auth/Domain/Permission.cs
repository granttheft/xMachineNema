using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Auth.Domain;

public sealed class Permission : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ModuleCode { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

