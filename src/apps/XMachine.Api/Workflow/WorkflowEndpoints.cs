using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Workflow;

public static class WorkflowEndpoints
{
    public static void MapWorkflowEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/workflow").RequireAuthorization();

        g.MapGet("definitions", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.WorkflowDefinitions.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.WorkflowType).ThenBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.TenantId,
                    x.WorkflowType,
                    x.Name,
                    x.Status,
                    StepCount = db.WorkflowSteps.Count(s => s.WorkflowDefinitionId == x.Id && s.TenantId == tenantId),
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("instances", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.WorkflowInstances.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.StartedAt)
                .Select(x => new
                {
                    x.Id,
                    x.TenantId,
                    x.WorkflowDefinitionId,
                    x.ReferenceType,
                    x.ReferenceId,
                    x.WorkflowState,
                    x.StartedAt,
                    x.EndedAt,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("summary", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var definitions = await db.WorkflowDefinitions.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var steps = await db.WorkflowSteps.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var instances = await db.WorkflowInstances.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var actions = await db.WorkflowActions.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            return Results.Ok(new { definitions, steps, instances, actions });
        });
    }
}
