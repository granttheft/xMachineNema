using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Platform;

public static class PlatformEndpoints
{
    public static void MapPlatformEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/platform").RequireAuthorization(AuthPolicies.AdminArea);

        g.MapGet("tenants", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Tenants.AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(t => new
                {
                    t.Id,
                    t.Code,
                    t.Name,
                    Status = (int)t.Status,
                    SiteCount = db.Sites.Count(s => s.TenantId == t.Id),
                    LineCount = db.Lines.Count(l => l.TenantId == t.Id),
                    MachineCount = db.Machines.Count(m => m.TenantId == t.Id),
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("sites", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Sites.AsNoTracking()
                .OrderBy(x => x.TenantId).ThenBy(x => x.Code)
                .Select(x => new
                {
                    x.Id,
                    x.TenantId,
                    x.EnterpriseId,
                    x.Code,
                    x.Name,
                    Status = (int)x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("lines", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Lines.AsNoTracking()
                .OrderBy(x => x.TenantId).ThenBy(x => x.Code)
                .Select(x => new
                {
                    x.Id,
                    x.TenantId,
                    x.SiteId,
                    x.BuildingId,
                    x.Code,
                    x.Name,
                    Status = (int)x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("machines", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Machines.AsNoTracking()
                .OrderBy(x => x.TenantId).ThenBy(x => x.Code)
                .Select(x => new
                {
                    x.Id,
                    x.TenantId,
                    x.LineId,
                    x.Code,
                    x.Name,
                    Status = (int)x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("summary", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var tenants = await db.Tenants.AsNoTracking().CountAsync(ct);
            var enterprises = await db.Enterprises.AsNoTracking().CountAsync(ct);
            var sites = await db.Sites.AsNoTracking().CountAsync(ct);
            var lines = await db.Lines.AsNoTracking().CountAsync(ct);
            var machines = await db.Machines.AsNoTracking().CountAsync(ct);
            var buildings = await db.Buildings.AsNoTracking().CountAsync(ct);
            var stations = await db.Stations.AsNoTracking().CountAsync(ct);
            return Results.Ok(new { tenants, enterprises, sites, lines, machines, buildings, stations });
        });
    }
}
