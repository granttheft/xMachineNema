using Microsoft.EntityFrameworkCore;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Mes;

public static class MesEndpoints
{
    public static void MapMesEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/mes");

        g.MapGet("production-orders", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.ProductionOrders.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.OrderNo,
                    x.ProductCode,
                    x.OrderStatus,
                    x.QuantityPlanned,
                    x.QuantityCompleted,
                    x.LineId,
                    x.SiteId,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("recipes", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Recipes.AsNoTracking()
                .OrderBy(x => x.Code)
                .ThenByDescending(x => x.Version)
                .Select(x => new { x.Id, x.Code, x.Name, x.Version, x.Status })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("lots", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.LotBatches.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.LotNo,
                    x.ProductionOrderId,
                    x.LotStatus,
                    x.QuantityGood,
                    x.LineId,
                    x.MachineId,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("shifts", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Shifts.AsNoTracking()
                .OrderByDescending(x => x.ShiftDate)
                .ThenBy(x => x.Code)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    x.ShiftDate,
                    x.SiteId,
                    x.LineId,
                    x.Lifecycle,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("summary", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var orders = await db.ProductionOrders.AsNoTracking().CountAsync(ct);
            var recipes = await db.Recipes.AsNoTracking().CountAsync(ct);
            var lots = await db.LotBatches.AsNoTracking().CountAsync(ct);
            var shifts = await db.Shifts.AsNoTracking().CountAsync(ct);
            return Results.Ok(new { orders, recipes, lots, shifts });
        });
    }
}
