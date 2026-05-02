namespace XMachine.Web.Services;

/// <summary>JSON shape for GET /api/planning/plans and /api/planning/plans/active.</summary>
public sealed record ApiProductionPlan(
    Guid Id,
    string PlanNo,
    string Name,
    string? Description,
    string PlanStatus,
    string Priority,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt,
    Guid? SiteId,
    Guid? LineId,
    Guid CreatedByUserId,
    Guid? ApprovedByUserId,
    DateTimeOffset? ApprovedAt,
    string? ApprovalNotes,
    string? CancellationReason,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid? CreatedBy,
    Guid? UpdatedBy);

/// <summary>JSON shape for GET /api/planning/slots.</summary>
public sealed record ApiPlanSlot(
    Guid Id,
    Guid ProductionPlanId,
    Guid MachineId,
    Guid? ProductionOrderId,
    Guid? JobExecutionId,
    string SlotNo,
    string SlotStatus,
    string Priority,
    int PlannedQty,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt,
    int? EstimatedDurationMinutes,
    int? SetupTimeMinutes,
    string? Notes,
    int SortOrder,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid? CreatedBy,
    Guid? UpdatedBy);

/// <summary>JSON shape for GET /api/planning/slots/by-machine/{machineId}.</summary>
public sealed record ApiPlanSlotWithPlan(
    Guid Id,
    Guid ProductionPlanId,
    Guid MachineId,
    Guid? ProductionOrderId,
    Guid? JobExecutionId,
    string SlotNo,
    string SlotStatus,
    string Priority,
    int PlannedQty,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt,
    int? EstimatedDurationMinutes,
    int? SetupTimeMinutes,
    string? Notes,
    int SortOrder,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid? CreatedBy,
    Guid? UpdatedBy,
    string PlanNo,
    string PlanName,
    string PlanStatus);

/// <summary>JSON shape for GET /api/planning/plans/{id}.</summary>
public sealed record ApiPlanWithSlots(ApiProductionPlan Plan, List<ApiPlanSlot> Slots);

/// <summary>JSON shape for GET /api/planning/summary.</summary>
public sealed record ApiPlanningSummary(
    int TotalPlans,
    int DraftPlans,
    int ApprovedPlans,
    int InProgressPlans,
    int CompletedPlans,
    int ScheduledSlotsToday,
    int TotalSlotsThisWeek);

/// <summary>Request body for POST /api/planning/plans.</summary>
public sealed record CreatePlanDto(
    string Name,
    string? Description,
    string Priority,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt,
    Guid? SiteId,
    Guid? LineId,
    string? Notes);

/// <summary>Request body for PUT /api/planning/plans/{id}/status.</summary>
public sealed record UpdatePlanStatusDto(string NewStatus, string? ApprovalNotes, string? CancellationReason);

/// <summary>Request body for POST /api/planning/slots.</summary>
public sealed record CreateSlotDto(
    Guid ProductionPlanId,
    Guid MachineId,
    Guid? ProductionOrderId,
    string Priority,
    int PlannedQty,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt,
    int? EstimatedDurationMinutes,
    int? SetupTimeMinutes,
    string? Notes);

/// <summary>Request body for PUT /api/planning/slots/{id}.</summary>
public sealed record UpdateSlotDto(
    Guid? MachineId,
    DateTimeOffset? PlannedStartAt,
    DateTimeOffset? PlannedEndAt,
    int? PlannedQty,
    string? NewStatus,
    string? Notes);

/// <summary>Response from POST /api/planning/plans.</summary>
public sealed record CreatePlanResponseDto(Guid Id, string PlanNo, string PlanStatus);

/// <summary>Response from PUT /api/planning/plans/{id}/status.</summary>
public sealed record UpdatePlanStatusResponseDto(Guid Id, string PlanNo, string PlanStatus);

/// <summary>Response from POST /api/planning/slots.</summary>
public sealed record CreateSlotResponseDto(Guid Id, string SlotNo, string SlotStatus);

/// <summary>Response from PUT /api/planning/slots/{id}.</summary>
public sealed record UpdateSlotResponseDto(
    Guid Id,
    string SlotNo,
    string SlotStatus,
    Guid MachineId,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt);
