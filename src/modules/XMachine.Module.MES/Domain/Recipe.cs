using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class Recipe : TenantAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    /// <summary>Monotonic recipe revision for the same <see cref="Code"/>.</summary>
    public int Version { get; set; } = 1;
    public string? Description { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
