using Microsoft.EntityFrameworkCore;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Eventing;

public static class EventingEndpoints
{
    public static void MapEventingEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/eventing");

        g.MapGet("alarms", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.AlarmEvents.AsNoTracking()
                .OrderByDescending(x => x.StartTime)
                .Select(x => new
                {
                    x.Id,
                    x.AlarmCode,
                    x.AlarmText,
                    x.Severity,
                    x.Category,
                    x.StartTime,
                    x.EndTime,
                    x.DurationMs,
                    x.SiteId,
                    x.LineId,
                    x.MachineId,
                    x.AckBy,
                    x.AckTime,
                    x.AlarmStatus,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("downtimes", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.DowntimeRecords.AsNoTracking()
                .OrderByDescending(x => x.StartTime)
                .Select(x => new
                {
                    x.Id,
                    x.DowntimeReasonCode,
                    x.DowntimeReasonText,
                    x.PlannedFlag,
                    x.StartTime,
                    x.EndTime,
                    x.DurationMs,
                    x.MachineId,
                    x.LineId,
                    x.ProductionOrderId,
                    x.EnteredBy,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("oee", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.OeeSnapshots.AsNoTracking()
                .OrderByDescending(x => x.PeriodStart)
                .Select(x => new
                {
                    x.Id,
                    x.PeriodType,
                    x.PeriodStart,
                    x.PeriodEnd,
                    x.SiteId,
                    x.LineId,
                    x.MachineId,
                    x.Availability,
                    x.Performance,
                    x.Quality,
                    x.OeeValue,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("kpis", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var defs = await db.KpiDefinitions.AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new { x.Id, x.Code, x.Name, x.ScopeType, x.DataSourceType, x.Status })
                .ToListAsync(ct);
            var results = await db.KpiResults.AsNoTracking()
                .OrderByDescending(x => x.PeriodStart)
                .Select(x => new
                {
                    x.Id,
                    x.KpiDefinitionId,
                    x.ScopeType,
                    x.ScopeId,
                    x.PeriodStart,
                    x.PeriodEnd,
                    x.Value,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(new { definitions = defs, results });
        });

        g.MapGet("summary", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var alarms = await db.AlarmEvents.AsNoTracking().CountAsync(ct);
            var downtimes = await db.DowntimeRecords.AsNoTracking().CountAsync(ct);
            var oee = await db.OeeSnapshots.AsNoTracking().CountAsync(ct);
            var kpiDefs = await db.KpiDefinitions.AsNoTracking().CountAsync(ct);
            var kpiResults = await db.KpiResults.AsNoTracking().CountAsync(ct);
            return Results.Ok(new { alarms, downtimes, oee, kpiDefs, kpiResults });
        });
    }
}
