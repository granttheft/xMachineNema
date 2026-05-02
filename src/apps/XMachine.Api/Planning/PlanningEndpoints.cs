using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Module.Planning.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Api.Planning;

/// <summary>Request body for POST /api/planning/plans.</summary>
public sealed record CreatePlanRequest(
    string Name,
    string? Description,
    PlanPriority Priority,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt,
    Guid? SiteId,
    Guid? LineId,
    string? Notes);

/// <summary>Request body for PUT /api/planning/plans/{id}/status.</summary>
public sealed record UpdatePlanStatusRequest(
    string NewStatus,
    string? ApprovalNotes,
    string? CancellationReason);

/// <summary>Request body for POST /api/planning/slots.</summary>
public sealed record CreateSlotRequest(
    Guid ProductionPlanId,
    Guid MachineId,
    Guid? ProductionOrderId,
    PlanPriority Priority,
    int PlannedQty,
    DateTimeOffset PlannedStartAt,
    DateTimeOffset PlannedEndAt,
    int? EstimatedDurationMinutes,
    int? SetupTimeMinutes,
    string? Notes);

/// <summary>Request body for PUT /api/planning/slots/{id}.</summary>
public sealed record UpdateSlotRequest(
    Guid? MachineId,
    DateTimeOffset? PlannedStartAt,
    DateTimeOffset? PlannedEndAt,
    int? PlannedQty,
    PlanSlotStatus? NewStatus,
    string? Notes);

/// <summary>Maps planning endpoints under <c>/api/planning</c> (reads and writes).</summary>
public static class PlanningEndpoints
{
    public static void MapPlanningEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/planning").RequireAuthorization();

        g.MapGet("plans", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.ProductionPlans.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.PlannedStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.PlanNo,
                    x.Name,
                    x.Description,
                    x.PlanStatus,
                    x.Priority,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.SiteId,
                    x.LineId,
                    x.CreatedByUserId,
                    x.ApprovedByUserId,
                    x.ApprovedAt,
                    x.ApprovalNotes,
                    x.CancellationReason,
                    x.Status,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.CreatedBy,
                    x.UpdatedBy,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("plans/active", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.ProductionPlans.AsNoTracking()
                .Where(x => x.TenantId == tenantId &&
                            (x.PlanStatus == PlanStatus.Submitted ||
                             x.PlanStatus == PlanStatus.Approved ||
                             x.PlanStatus == PlanStatus.InProgress))
                .OrderBy(x => x.PlannedStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.PlanNo,
                    x.Name,
                    x.Description,
                    x.PlanStatus,
                    x.Priority,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.SiteId,
                    x.LineId,
                    x.CreatedByUserId,
                    x.ApprovedByUserId,
                    x.ApprovedAt,
                    x.ApprovalNotes,
                    x.CancellationReason,
                    x.Status,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.CreatedBy,
                    x.UpdatedBy,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("plans/{id:guid}", async (Guid id, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var plan = await db.ProductionPlans.AsNoTracking()
                .Where(x => x.Id == id && x.TenantId == tenantId)
                .Select(x => new
                {
                    x.Id,
                    x.PlanNo,
                    x.Name,
                    x.Description,
                    x.PlanStatus,
                    x.Priority,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.SiteId,
                    x.LineId,
                    x.CreatedByUserId,
                    x.ApprovedByUserId,
                    x.ApprovedAt,
                    x.ApprovalNotes,
                    x.CancellationReason,
                    x.Status,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.CreatedBy,
                    x.UpdatedBy,
                })
                .FirstOrDefaultAsync(ct);
            if (plan is null)
                return Results.NotFound();

            var slots = await db.PlanSlots.AsNoTracking()
                .Where(s => s.ProductionPlanId == id && s.TenantId == tenantId)
                .OrderBy(s => s.SortOrder)
                .Select(s => new
                {
                    s.Id,
                    s.ProductionPlanId,
                    s.MachineId,
                    s.ProductionOrderId,
                    s.JobExecutionId,
                    s.SlotNo,
                    s.SlotStatus,
                    s.Priority,
                    s.PlannedQty,
                    s.PlannedStartAt,
                    s.PlannedEndAt,
                    s.EstimatedDurationMinutes,
                    s.SetupTimeMinutes,
                    s.Notes,
                    s.SortOrder,
                    s.Status,
                    s.CreatedAt,
                    s.UpdatedAt,
                    s.CreatedBy,
                    s.UpdatedBy,
                })
                .ToListAsync(ct);

            return Results.Ok(new { plan, slots });
        });

        g.MapGet("slots", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.PlanSlots.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.PlannedStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.ProductionPlanId,
                    x.MachineId,
                    x.ProductionOrderId,
                    x.JobExecutionId,
                    x.SlotNo,
                    x.SlotStatus,
                    x.Priority,
                    x.PlannedQty,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.EstimatedDurationMinutes,
                    x.SetupTimeMinutes,
                    x.Notes,
                    x.SortOrder,
                    x.Status,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.CreatedBy,
                    x.UpdatedBy,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("slots/by-machine/{machineId:guid}", async (
            Guid machineId,
            XMachineDbContext db,
            ICurrentUser currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.PlanSlots.AsNoTracking()
                .Where(s => s.TenantId == tenantId && s.MachineId == machineId)
                .Join(
                    db.ProductionPlans.AsNoTracking().Where(p => p.TenantId == tenantId),
                    s => s.ProductionPlanId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.Id,
                        s.ProductionPlanId,
                        s.MachineId,
                        s.ProductionOrderId,
                        s.JobExecutionId,
                        s.SlotNo,
                        s.SlotStatus,
                        s.Priority,
                        s.PlannedQty,
                        s.PlannedStartAt,
                        s.PlannedEndAt,
                        s.EstimatedDurationMinutes,
                        s.SetupTimeMinutes,
                        s.Notes,
                        s.SortOrder,
                        s.Status,
                        s.CreatedAt,
                        s.UpdatedAt,
                        s.CreatedBy,
                        s.UpdatedBy,
                        planNo = p.PlanNo,
                        planName = p.Name,
                        planStatus = p.PlanStatus,
                    })
                .OrderBy(x => x.PlannedStartAt)
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("summary", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var totalPlans = await db.ProductionPlans.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId, ct);
            var draftPlans = await db.ProductionPlans.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.PlanStatus == PlanStatus.Draft, ct);
            var approvedPlans = await db.ProductionPlans.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.PlanStatus == PlanStatus.Approved, ct);
            var inProgressPlans = await db.ProductionPlans.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.PlanStatus == PlanStatus.InProgress, ct);
            var completedPlans = await db.ProductionPlans.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.PlanStatus == PlanStatus.Done, ct);

            var todayStart = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
            var todayEnd = todayStart.AddDays(1);

            var scheduledSlotsToday = await db.PlanSlots.AsNoTracking()
                .CountAsync(
                    x => x.TenantId == tenantId &&
                         x.SlotStatus == PlanSlotStatus.Scheduled &&
                         x.PlannedStartAt >= todayStart &&
                         x.PlannedStartAt < todayEnd,
                    ct);

            var mondayUtc = StartOfWeekMondayUtc(DateTime.UtcNow);
            var weekEndExclusive = mondayUtc.AddDays(7);

            var totalSlotsThisWeek = await db.PlanSlots.AsNoTracking()
                .CountAsync(
                    x => x.TenantId == tenantId &&
                         x.PlannedStartAt >= mondayUtc &&
                         x.PlannedStartAt < weekEndExclusive,
                    ct);

            return Results.Ok(new
            {
                totalPlans,
                draftPlans,
                approvedPlans,
                inProgressPlans,
                completedPlans,
                scheduledSlotsToday,
                totalSlotsThisWeek,
            });
        });

        g.MapPost("plans", async (CreatePlanRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            if (string.IsNullOrWhiteSpace(body.Name))
                return Results.BadRequest("Name is required");
            if (body.PlannedEndAt <= body.PlannedStartAt)
                return Results.BadRequest("End must be after start");

            var todayPrefix = $"PLAN-{DateTime.UtcNow:yyyyMMdd}";
            var count = await db.ProductionPlans.CountAsync(
                p => p.TenantId == tenantId && p.PlanNo.StartsWith(todayPrefix),
                ct);
            var planNo = $"{todayPrefix}-{(count + 1):D3}";

            var userId = currentUser.UserId ?? Guid.Empty;
            var now = DateTimeOffset.UtcNow;
            var description = string.IsNullOrWhiteSpace(body.Description) ? null : body.Description.Trim();
            if (!string.IsNullOrWhiteSpace(body.Notes))
            {
                var notes = body.Notes.Trim();
                description = string.IsNullOrWhiteSpace(description) ? notes : $"{description}\n{notes}";
            }
            var plan = new ProductionPlan
            {
                TenantId = tenantId,
                PlanNo = planNo,
                Name = body.Name.Trim(),
                Description = description,
                Priority = body.Priority,
                PlanStatus = PlanStatus.Draft,
                PlannedStartAt = body.PlannedStartAt,
                PlannedEndAt = body.PlannedEndAt,
                SiteId = body.SiteId,
                LineId = body.LineId,
                CreatedByUserId = userId,
                Status = EntityStatus.Active,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = currentUser.UserId,
                UpdatedBy = currentUser.UserId,
            };
            db.ProductionPlans.Add(plan);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/planning/plans/{plan.Id}", new { plan.Id, plan.PlanNo, plan.PlanStatus });
        });

        g.MapPut("plans/{id:guid}/status", async (
            Guid id,
            UpdatePlanStatusRequest body,
            XMachineDbContext db,
            ICurrentUser currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var plan = await db.ProductionPlans.FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, ct);
            if (plan is null)
                return Results.NotFound();

            if (!Enum.TryParse<PlanStatus>(body.NewStatus, ignoreCase: true, out var newStatus))
                return Results.BadRequest($"Invalid plan status: {body.NewStatus}");

            if (!IsAllowedPlanStatusTransition(plan.PlanStatus, newStatus))
                return Results.BadRequest($"Invalid status transition from {plan.PlanStatus} to {newStatus}");

            plan.PlanStatus = newStatus;
            if (newStatus == PlanStatus.Approved)
            {
                plan.ApprovedByUserId = currentUser.UserId;
                plan.ApprovedAt = DateTimeOffset.UtcNow;
                plan.ApprovalNotes = string.IsNullOrWhiteSpace(body.ApprovalNotes) ? null : body.ApprovalNotes.Trim();
            }
            if (newStatus == PlanStatus.Cancelled)
                plan.CancellationReason = string.IsNullOrWhiteSpace(body.CancellationReason) ? null : body.CancellationReason.Trim();

            var now = DateTimeOffset.UtcNow;
            plan.UpdatedAt = now;
            plan.UpdatedBy = currentUser.UserId;
            await db.SaveChangesAsync(ct);

            return Results.Ok(new { plan.Id, plan.PlanNo, plan.PlanStatus });
        });

        g.MapPost("slots", async (CreateSlotRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var plan = await db.ProductionPlans.FirstOrDefaultAsync(
                p => p.Id == body.ProductionPlanId && p.TenantId == tenantId,
                ct);
            if (plan is null)
                return Results.NotFound("Plan not found");

            if (plan.PlanStatus is PlanStatus.Done or PlanStatus.Cancelled)
                return Results.BadRequest("Cannot add slots to a closed plan");

            if (body.PlannedEndAt <= body.PlannedStartAt)
                return Results.BadRequest("End must be after start");

            var slotCount = await db.PlanSlots.CountAsync(
                s => s.ProductionPlanId == body.ProductionPlanId && s.TenantId == tenantId,
                ct);
            var slotNo = $"SLOT-{(slotCount + 1):D3}";

            var now = DateTimeOffset.UtcNow;
            var slot = new PlanSlot
            {
                TenantId = tenantId,
                ProductionPlanId = body.ProductionPlanId,
                MachineId = body.MachineId,
                ProductionOrderId = body.ProductionOrderId,
                SlotNo = slotNo,
                SlotStatus = PlanSlotStatus.Scheduled,
                Priority = body.Priority,
                PlannedQty = body.PlannedQty,
                PlannedStartAt = body.PlannedStartAt,
                PlannedEndAt = body.PlannedEndAt,
                EstimatedDurationMinutes = body.EstimatedDurationMinutes,
                SetupTimeMinutes = body.SetupTimeMinutes,
                Notes = string.IsNullOrWhiteSpace(body.Notes) ? null : body.Notes.Trim(),
                SortOrder = slotCount + 1,
                Status = EntityStatus.Active,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = currentUser.UserId,
                UpdatedBy = currentUser.UserId,
            };
            db.PlanSlots.Add(slot);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/planning/slots/{slot.Id}", new { slot.Id, slot.SlotNo, slot.SlotStatus });
        });

        g.MapPut("slots/{id:guid}", async (
            Guid id,
            UpdateSlotRequest body,
            XMachineDbContext db,
            ICurrentUser currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var slot = await db.PlanSlots.FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId, ct);
            if (slot is null)
                return Results.NotFound();

            if (slot.SlotStatus is PlanSlotStatus.Done or PlanSlotStatus.Cancelled)
                return Results.BadRequest("Cannot modify a closed slot");

            var mergedStart = body.PlannedStartAt ?? slot.PlannedStartAt;
            var mergedEnd = body.PlannedEndAt ?? slot.PlannedEndAt;
            if (mergedEnd <= mergedStart)
                return Results.BadRequest("End must be after start");

            if (body.MachineId is not null)
                slot.MachineId = body.MachineId.Value;
            if (body.PlannedStartAt is not null)
                slot.PlannedStartAt = body.PlannedStartAt.Value;
            if (body.PlannedEndAt is not null)
                slot.PlannedEndAt = body.PlannedEndAt.Value;
            if (body.PlannedQty is not null)
                slot.PlannedQty = body.PlannedQty.Value;
            if (body.NewStatus is not null)
                slot.SlotStatus = body.NewStatus.Value;
            if (body.Notes is not null)
                slot.Notes = body.Notes;

            var now = DateTimeOffset.UtcNow;
            slot.UpdatedAt = now;
            slot.UpdatedBy = currentUser.UserId;
            await db.SaveChangesAsync(ct);

            return Results.Ok(new
            {
                slot.Id,
                slot.SlotNo,
                slot.SlotStatus,
                slot.MachineId,
                slot.PlannedStartAt,
                slot.PlannedEndAt,
            });
        });
    }

    /// <summary>Returns whether a plan may move from <paramref name="from"/> to <paramref name="to"/>.</summary>
    private static bool IsAllowedPlanStatusTransition(PlanStatus from, PlanStatus to)
    {
        if (from == to)
            return false;
        return from switch
        {
            PlanStatus.Draft => to is PlanStatus.Submitted or PlanStatus.Approved or PlanStatus.Cancelled,
            PlanStatus.Submitted => to is PlanStatus.Approved or PlanStatus.Cancelled,
            PlanStatus.Approved => to is PlanStatus.InProgress or PlanStatus.Cancelled,
            PlanStatus.InProgress => to is PlanStatus.Done or PlanStatus.Cancelled,
            PlanStatus.Done => false,
            PlanStatus.Cancelled => false,
            _ => false,
        };
    }

    private static DateTimeOffset StartOfWeekMondayUtc(DateTime utcNow)
    {
        var date = utcNow.Date;
        var day = utcNow.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)utcNow.DayOfWeek;
        var daysSinceMonday = day - (int)DayOfWeek.Monday;
        var monday = date.AddDays(-daysSinceMonday);
        return new DateTimeOffset(monday, TimeSpan.Zero);
    }
}
