using Microsoft.EntityFrameworkCore;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Quality;

public static class QualityEndpoints
{
    public static void MapQualityEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/quality");

        g.MapGet("checks", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.QualityChecks.AsNoTracking()
                .OrderByDescending(x => x.CheckTime)
                .Select(x => new
                {
                    x.Id,
                    x.CheckType,
                    x.CheckTime,
                    x.CheckStatus,
                    x.ProductionOrderId,
                    x.LotBatchId,
                    x.MachineId,
                    x.ApprovedBy,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("nonconformances", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Nonconformances.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.QualityCheckId,
                    x.NcCode,
                    x.Description,
                    x.Severity,
                    x.NcStatus,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("summary", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var checks = await db.QualityChecks.AsNoTracking().CountAsync(ct);
            var measurements = await db.QualityMeasurements.AsNoTracking().CountAsync(ct);
            var nonconformances = await db.Nonconformances.AsNoTracking().CountAsync(ct);
            var dispositions = await db.QualityDispositions.AsNoTracking().CountAsync(ct);
            return Results.Ok(new { checks, measurements, nonconformances, dispositions });
        });
    }
}
