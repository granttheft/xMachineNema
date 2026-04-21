using Microsoft.EntityFrameworkCore;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Workflow;

public static class WorkflowEndpoints
{
    public static void MapWorkflowEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/workflow");

        g.MapGet("definitions", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.WorkflowDefinitions.AsNoTracking()
                .OrderBy(x => x.WorkflowType).ThenBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.TenantId,
                    x.WorkflowType,
                    x.Name,
                    x.Status,
                    StepCount = db.WorkflowSteps.Count(s => s.WorkflowDefinitionId == x.Id),
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("instances", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var rows = await db.WorkflowInstances.AsNoTracking()
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

        g.MapGet("summary", async (XMachineDbContext db, CancellationToken ct) =>
        {
            var definitions = await db.WorkflowDefinitions.AsNoTracking().CountAsync(ct);
            var steps = await db.WorkflowSteps.AsNoTracking().CountAsync(ct);
            var instances = await db.WorkflowInstances.AsNoTracking().CountAsync(ct);
            var actions = await db.WorkflowActions.AsNoTracking().CountAsync(ct);
            return Results.Ok(new { definitions, steps, instances, actions });
        });
    }
}
