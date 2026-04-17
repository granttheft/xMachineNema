using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class OrderRecipeAssignment : TenantAuditableEntity
{
    public Guid ProductionOrderId { get; set; }
    public Guid RecipeId { get; set; }

    /// <summary>Snapshot of <see cref="Recipe.Version"/> at assignment time.</summary>
    public int RecipeVersionAssigned { get; set; }

    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsPrimary { get; set; } = true;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
