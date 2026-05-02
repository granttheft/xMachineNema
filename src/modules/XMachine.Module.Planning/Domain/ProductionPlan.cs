using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Planning.Domain;

/// <summary>
/// A production plan covering one or more machines and time slots.
/// </summary>
public sealed class ProductionPlan : TenantAuditableEntity
{
    /// <summary>Human-readable plan number (e.g. PLAN-yyyyMMdd-NNN).</summary>
    public string PlanNo { get; set; } = string.Empty;

    /// <summary>Short descriptive title for the plan.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional long-form description.</summary>
    public string? Description { get; set; }

    public PlanStatus PlanStatus { get; set; } = PlanStatus.Draft;

    public PlanPriority Priority { get; set; } = PlanPriority.Medium;

    public DateTimeOffset PlannedStartAt { get; set; }

    public DateTimeOffset PlannedEndAt { get; set; }

    public Guid? SiteId { get; set; }

    public Guid? LineId { get; set; }

    /// <summary>User who created the plan.</summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>User who approved the plan, if applicable.</summary>
    public Guid? ApprovedByUserId { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public string? ApprovalNotes { get; set; }

    public string? CancellationReason { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
