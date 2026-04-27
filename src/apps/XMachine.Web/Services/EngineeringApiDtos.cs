namespace XMachine.Web.Services;

/// <summary>
/// Request body for POST /api/engineering/work-orders.
/// </summary>
public sealed record CreateWorkOrderDto(
    Guid MachineId,
    Guid? LineId,
    string WorkOrderType,
    string Priority,
    string Description,
    string? ReasonCode,
    string? AssignedRole);

/// <summary>
/// Response body for POST /api/engineering/work-orders.
/// </summary>
public sealed record CreateWorkOrderResponse(Guid Id, string WorkOrderNo);

/// <summary>
/// Request body for POST /api/engineering/pm-schedules.
/// </summary>
public sealed record CreatePmScheduleDto(
    Guid MachineId,
    string Name,
    string? Description,
    int? IntervalHours,
    int? IntervalDays,
    DateTimeOffset? FirstDueAt,
    string? OwnerRole);

/// <summary>
/// Response body for POST /api/engineering/pm-schedules.
/// </summary>
public sealed record CreatePmScheduleResponse(Guid Id, string Name);

/// <summary>
/// Request body for PUT /api/engineering/work-orders/{id}/status.
/// </summary>
public sealed record UpdateWorkOrderStatusDto(string NewStatus, string? ClosingNotes);

/// <summary>
/// Response body for PUT /api/engineering/work-orders/{id}/status.
/// </summary>
public sealed record UpdateWorkOrderStatusResponse(Guid Id, string WorkOrderNo, string WorkOrderStatus);

/// <summary>
/// Request body for PUT /api/engineering/machine-status/{id}.
/// </summary>
public sealed record UpdateMachineStatusDto(string OperationalStatus);

/// <summary>
/// Response body for PUT /api/engineering/machine-status/{id}.
/// </summary>
public sealed record UpdateMachineStatusResponse(Guid Id, string Code, string OperationalStatus);

/// <summary>
/// Request body for POST /api/engineering/faults.
/// </summary>
public sealed record CreateFaultDto(
    Guid MachineId,
    Guid? MaintenanceWorkOrderId,
    string FaultCode,
    string Description);

/// <summary>
/// Response body for POST /api/engineering/faults.
/// </summary>
public sealed record CreateFaultResponse(Guid Id, string FaultCode);

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
