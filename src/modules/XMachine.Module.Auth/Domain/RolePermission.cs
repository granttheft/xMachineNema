using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Auth.Domain;

public sealed class RolePermission : AuditableEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

