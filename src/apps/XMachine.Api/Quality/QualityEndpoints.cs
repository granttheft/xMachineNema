using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Quality;

public static class QualityEndpoints
{
    public static void MapQualityEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/quality").RequireAuthorization();

        g.MapGet("checks", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.QualityChecks.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
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

        g.MapGet("nonconformances", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.Nonconformances.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
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

        g.MapGet("summary", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var checks = await db.QualityChecks.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var measurements = await db.QualityMeasurements.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var nonconformances = await db.Nonconformances.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var dispositions = await db.QualityDispositions.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            return Results.Ok(new { checks, measurements, nonconformances, dispositions });
        });
    }
}
