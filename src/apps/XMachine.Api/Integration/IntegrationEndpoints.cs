using Microsoft.EntityFrameworkCore;
using XMachine.Connectors.Abstractions;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Integration;

public static class IntegrationEndpoints
{
    public static void MapIntegrationEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/integration");

        g.MapGet("connector-definitions", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.ConnectorDefinitions.AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new { x.Id, x.Code, x.Name, x.Category, x.Direction, x.Status })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("connector-instances", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.ConnectorInstances.AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new { x.Id, x.Code, x.Name, x.ConnectorDefinitionId, x.SiteId, x.Status })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("mapping-profiles", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.MappingProfiles.AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenByDescending(x => x.Version)
                .Select(x => new { x.Id, x.Name, x.Version, x.ConnectorInstanceId, x.Status })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("health/summary", async (
            XMachineDbContext db,
            IConnectorRegistry registry,
            IConnectorHealthProvider health,
            CancellationToken ct) =>
        {
            var definitions = await db.ConnectorDefinitions.AsNoTracking().CountAsync(ct);
            var instances = await db.ConnectorInstances.AsNoTracking().CountAsync(ct);
            var profiles = await db.MappingProfiles.AsNoTracking().CountAsync(ct);

            var runtime = await health.GetSnapshotAsync(ct).ConfigureAwait(false);

            return Results.Ok(new
            {
                operational = new { definitions, instances, profiles },
                runtime = new
                {
                    registryCodes = registry.RegisteredCodes,
                    activeHealthRows = runtime.Count,
                },
            });
        });
    }
}
