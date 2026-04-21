using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Commercial;

public static class CommercialEndpoints
{
    public static void MapCommercialEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/commercial").RequireAuthorization(AuthPolicies.AdminArea);

        g.MapGet("licenses", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Licenses.AsNoTracking()
                .OrderBy(x => x.TenantId).ThenBy(x => x.ValidFrom)
                .Select(x => new
                {
                    x.Id,
                    x.TenantId,
                    x.LicenseType,
                    x.ValidFrom,
                    x.ValidTo,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("modules", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.Modules.AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("module-activations", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await (
                    from a in db.TenantModuleActivations.AsNoTracking()
                    join m in db.Modules.AsNoTracking() on a.ModuleId equals m.Id
                    orderby a.TenantId, m.Code
                    select new
                    {
                        a.Id,
                        a.TenantId,
                        a.ModuleId,
                        ModuleCode = m.Code,
                        ModuleName = m.Name,
                        a.ActivatedAt,
                        a.Status,
                    })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("licensed-lines", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await (
                    from ll in db.LicensedLines.AsNoTracking()
                    join ln in db.Lines.AsNoTracking() on ll.LineId equals ln.Id
                    orderby ll.TenantId, ln.Code
                    select new
                    {
                        ll.Id,
                        ll.TenantId,
                        ll.LineId,
                        LineCode = ln.Code,
                        LineName = ln.Name,
                        ll.Status,
                    })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("summary", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var licenses = await db.Licenses.AsNoTracking().CountAsync(ct);
            var modules = await db.Modules.AsNoTracking().CountAsync(ct);
            var moduleActivations = await db.TenantModuleActivations.AsNoTracking().CountAsync(ct);
            var licensedLines = await db.LicensedLines.AsNoTracking().CountAsync(ct);
            return Results.Ok(new { licenses, modules, moduleActivations, licensedLines });
        });
    }
}
