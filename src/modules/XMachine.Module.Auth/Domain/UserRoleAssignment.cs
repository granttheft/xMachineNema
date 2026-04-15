using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Auth.Domain;

public sealed class UserRoleAssignment : TenantAuditableEntity
{
    public Guid UserAccountId { get; set; }
    public Guid RoleId { get; set; }

    public ScopeType ScopeType { get; set; } = ScopeType.Tenant;
    public Guid? ScopeId { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

