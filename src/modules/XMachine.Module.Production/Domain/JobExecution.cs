using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Production.Domain;

public sealed class JobExecution : TenantAuditableEntity
{
    public Guid MachineId { get; set; }
    public Guid LineId { get; set; }
    public Guid ProductionOrderId { get; set; }
    public Guid? RecipeId { get; set; }
    public Guid? ShiftId { get; set; }
    public Guid? OperatorId { get; set; }

    public string JobNo { get; set; } = string.Empty;
    public JobExecutionStatus ExecutionStatus { get; set; }

    public int PlannedQty { get; set; }
    public int ProducedQty { get; set; }
    public int ScrapQty { get; set; }
    public int DefectQty { get; set; }

    public DateTimeOffset? PlannedStartAt { get; set; }
    public DateTimeOffset? PlannedEndAt { get; set; }
    public DateTimeOffset? ActualStartAt { get; set; }
    public DateTimeOffset? ActualEndAt { get; set; }
    public DateTimeOffset? PausedAt { get; set; }

    public JobPauseReason? PauseReason { get; set; }
    public string? PauseNotes { get; set; }
    public string? Notes { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
