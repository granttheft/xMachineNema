using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Planning.Domain;

/// <summary>
/// A single scheduled job slot within a <see cref="ProductionPlan"/> (one machine, product window).
/// </summary>
public sealed class PlanSlot : TenantAuditableEntity
{
    public Guid ProductionPlanId { get; set; }

    public Guid MachineId { get; set; }

    public Guid? ProductionOrderId { get; set; }

    /// <summary>Optional link to a live job when the slot is started.</summary>
    public Guid? JobExecutionId { get; set; }

    /// <summary>Slot identifier within the plan (e.g. SLOT-001).</summary>
    public string SlotNo { get; set; } = string.Empty;

    public PlanSlotStatus SlotStatus { get; set; } = PlanSlotStatus.Scheduled;

    public PlanPriority Priority { get; set; } = PlanPriority.Medium;

    public int PlannedQty { get; set; }

    public DateTimeOffset PlannedStartAt { get; set; }

    public DateTimeOffset PlannedEndAt { get; set; }

    public int? EstimatedDurationMinutes { get; set; }

    /// <summary>Optional setup or mold-change buffer in minutes.</summary>
    public int? SetupTimeMinutes { get; set; }

    public string? Notes { get; set; }

    /// <summary>Display order within the parent plan.</summary>
    public int SortOrder { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
