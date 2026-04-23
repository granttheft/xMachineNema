namespace XMachine.Web.Services;

/// <summary>
/// JSON shape for GET /api/engineering/machine-status.
/// </summary>
public sealed record ApiMachine(
    Guid Id,
    string Code,
    string Name,
    Guid LineId,
    string? MachineType,
    string? Location,
    string OperationalStatus);

/// <summary>
/// JSON shape for GET /api/engineering/work-orders.
/// </summary>
public sealed record ApiWorkOrder(
    Guid Id,
    string WorkOrderNo,
    Guid MachineId,
    Guid? LineId,
    string WorkOrderType,
    string Priority,
    string WorkOrderStatus,
    string Description,
    string? ReasonCode,
    Guid? AssignedTo,
    string? AssignedRole,
    DateTimeOffset ReportedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    string? ClosingNotes,
    string Status);

/// <summary>
/// JSON shape for GET /api/engineering/pm-schedules.
/// </summary>
public sealed record ApiPmSchedule(
    Guid Id,
    Guid MachineId,
    string Name,
    int? IntervalHours,
    int? IntervalDays,
    DateTimeOffset? LastDoneAt,
    DateTimeOffset? NextDueAt,
    string? OwnerRole,
    string Status);

/// <summary>
/// JSON shape for GET /api/engineering/summary (optional future use).
/// </summary>
public sealed record ApiSummary(
    int TotalMachines,
    int RunningMachines,
    int DownMachines,
    int PmDueMachines,
    int OpenWorkOrders,
    int InProgressWorkOrders,
    int UnresolvedFaults,
    int OverdueSchedules);
