using System.Text.Json.Serialization;

namespace XMachine.Web.Services;

/// <summary>
/// JSON shape for GET /api/production/jobs and /api/production/jobs/active.
/// </summary>
public sealed record ApiJobExecution(
    Guid Id,
    Guid MachineId,
    Guid LineId,
    Guid ProductionOrderId,
    Guid? RecipeId,
    Guid? ShiftId,
    Guid? OperatorId,
    string JobNo,
    string ExecutionStatus,
    int PlannedQty,
    int ProducedQty,
    int ScrapQty,
    int DefectQty,
    DateTimeOffset? PlannedStartAt,
    DateTimeOffset? PlannedEndAt,
    DateTimeOffset? ActualStartAt,
    DateTimeOffset? ActualEndAt,
    DateTimeOffset? PausedAt,
    string? PauseReason,
    string? PauseNotes,
    string? Notes,
    string Status);

/// <summary>
/// Nested active job on a machine from GET /api/production/machines.
/// </summary>
public sealed record ApiActiveJob(
    Guid Id,
    string JobNo,
    string ExecutionStatus,
    int ProducedQty,
    int PlannedQty,
    int ScrapQty,
    Guid? OperatorId,
    DateTimeOffset? ActualStartAt,
    string? PauseReason);

/// <summary>
/// JSON shape for GET /api/production/machines.
/// </summary>
public sealed record ApiMachineWithJob(
    Guid Id,
    string Code,
    string Name,
    Guid LineId,
    string OperationalStatus,
    [property: JsonPropertyName("activeJob")]
    ApiActiveJob? ActiveJob);

/// <summary>
/// JSON shape for GET /api/production/summary.
/// </summary>
public sealed record ApiProductionSummary(
    int TotalMachines,
    int RunningJobs,
    int PausedJobs,
    int QueuedJobs,
    int CompletedToday,
    int TotalProducedToday,
    int TotalScrapToday);
