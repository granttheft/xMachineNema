using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Module.Planning.Domain;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Planning;

/// <summary>Maps read-only planning endpoints under <c>/api/planning</c>.</summary>
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
