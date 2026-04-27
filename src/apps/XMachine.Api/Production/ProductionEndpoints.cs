using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Module.Production.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Api.Production;

public static class ProductionEndpoints
{
    public static void MapProductionEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/production").RequireAuthorization();

        g.MapGet("jobs", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.JobExecutions.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.ActualStartAt == null ? 1 : 0)
                .ThenByDescending(x => x.ActualStartAt)
                .ThenByDescending(x => x.PlannedStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.MachineId,
                    x.LineId,
                    x.ProductionOrderId,
                    x.RecipeId,
                    x.ShiftId,
                    x.OperatorId,
                    x.JobNo,
                    x.ExecutionStatus,
                    x.PlannedQty,
                    x.ProducedQty,
                    x.ScrapQty,
                    x.DefectQty,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.ActualStartAt,
                    x.ActualEndAt,
                    x.PausedAt,
                    x.PauseReason,
                    x.PauseNotes,
                    x.Notes,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("jobs/active", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.JobExecutions.AsNoTracking()
                .Where(x => x.TenantId == tenantId &&
                            (x.ExecutionStatus == JobExecutionStatus.Running ||
                             x.ExecutionStatus == JobExecutionStatus.Setup ||
                             x.ExecutionStatus == JobExecutionStatus.Paused))
                .OrderByDescending(x => x.ActualStartAt)
                .ThenByDescending(x => x.PlannedStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.MachineId,
                    x.LineId,
                    x.ProductionOrderId,
                    x.RecipeId,
                    x.ShiftId,
                    x.OperatorId,
                    x.JobNo,
                    x.ExecutionStatus,
                    x.PlannedQty,
                    x.ProducedQty,
                    x.ScrapQty,
                    x.DefectQty,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.ActualStartAt,
                    x.ActualEndAt,
                    x.PausedAt,
                    x.PauseReason,
                    x.PauseNotes,
                    x.Notes,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("machines", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var machines = await db.Machines.AsNoTracking()
                .Where(m => m.TenantId == tenantId && m.Status == EntityStatus.Active)
                .OrderBy(m => m.Code)
                .Select(m => new
                {
                    m.Id,
                    m.Code,
                    m.Name,
                    m.LineId,
                    m.OperationalStatus,
                })
                .ToListAsync(ct);

            var activeJobs = await db.JobExecutions.AsNoTracking()
                .Where(j => j.TenantId == tenantId &&
                            (j.ExecutionStatus == JobExecutionStatus.Running ||
                             j.ExecutionStatus == JobExecutionStatus.Setup ||
                             j.ExecutionStatus == JobExecutionStatus.Paused))
                .ToListAsync(ct);

            var latestByMachine = activeJobs
                .GroupBy(j => j.MachineId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.ActualStartAt ?? x.PlannedStartAt ?? DateTimeOffset.MinValue).First());

            var result = machines.Select(m =>
            {
                latestByMachine.TryGetValue(m.Id, out var job);
                return new
                {
                    m.Id,
                    m.Code,
                    m.Name,
                    m.LineId,
                    m.OperationalStatus,
                    activeJob = job is null
                        ? null
                        : new
                        {
                            job.Id,
                            job.JobNo,
                            job.ExecutionStatus,
                            job.ProducedQty,
                            job.PlannedQty,
                            job.ScrapQty,
                            job.OperatorId,
                            job.ActualStartAt,
                            job.PauseReason,
                        },
                };
            }).ToList();

            return Results.Ok(result);
        });

        g.MapGet("summary", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;
            var todayStart = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);

            var totalMachines = await db.Machines.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.Status == EntityStatus.Active, ct);

            var runningJobs = await db.JobExecutions.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.ExecutionStatus == JobExecutionStatus.Running, ct);

            var pausedJobs = await db.JobExecutions.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.ExecutionStatus == JobExecutionStatus.Paused, ct);

            var queuedJobs = await db.JobExecutions.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.ExecutionStatus == JobExecutionStatus.Queued, ct);

            var completedToday = await db.JobExecutions.AsNoTracking()
                .CountAsync(x =>
                    x.TenantId == tenantId &&
                    x.ExecutionStatus == JobExecutionStatus.Done &&
                    x.ActualEndAt != null &&
                    x.ActualEndAt >= todayStart, ct);

            var totalProducedToday = await db.JobExecutions.AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ExecutionStatus == JobExecutionStatus.Done &&
                    x.ActualEndAt != null &&
                    x.ActualEndAt >= todayStart)
                .SumAsync(x => (int?)x.ProducedQty, ct) ?? 0;

            var totalScrapToday = await db.JobExecutions.AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ExecutionStatus == JobExecutionStatus.Done &&
                    x.ActualEndAt != null &&
                    x.ActualEndAt >= todayStart)
                .SumAsync(x => (int?)x.ScrapQty, ct) ?? 0;

            return Results.Ok(new
            {
                totalMachines,
                runningJobs,
                pausedJobs,
                queuedJobs,
                completedToday,
                totalProducedToday,
                totalScrapToday,
            });
        });
    }
}
