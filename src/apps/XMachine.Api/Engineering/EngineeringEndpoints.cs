using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Module.Engineering.Domain;
using XMachine.Module.Platform.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Api.Engineering;

public record CreateWorkOrderRequest(
    Guid MachineId,
    Guid? LineId,
    MaintenanceWorkOrderType WorkOrderType,
    MaintenancePriority Priority,
    string Description,
    string? ReasonCode,
    string? AssignedRole);

public static class EngineeringEndpoints
{
    public static void MapEngineeringEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/engineering").RequireAuthorization();

        g.MapGet("machine-status", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.Machines.AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.Status == EntityStatus.Active)
                .OrderBy(x => x.Code)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    x.LineId,
                    x.MachineType,
                    x.Location,
                    x.OperationalStatus,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("work-orders", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.MaintenanceWorkOrders.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.ReportedAt)
                .Select(x => new
                {
                    x.Id,
                    x.WorkOrderNo,
                    x.MachineId,
                    x.LineId,
                    x.WorkOrderType,
                    x.Priority,
                    x.WorkOrderStatus,
                    x.Description,
                    x.ReasonCode,
                    x.AssignedTo,
                    x.AssignedRole,
                    x.ReportedAt,
                    x.ReportedBy,
                    x.StartedAt,
                    x.CompletedAt,
                    x.ClosingNotes,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapPost("work-orders", async (CreateWorkOrderRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var machine = await db.Machines
                .FirstOrDefaultAsync(m => m.Id == body.MachineId && m.TenantId == tenantId, ct);
            if (machine is null)
                return Results.NotFound();

            var wo = new MaintenanceWorkOrder
            {
                TenantId = tenantId,
                MachineId = body.MachineId,
                LineId = body.LineId,
                WorkOrderNo = $"WO-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                WorkOrderType = body.WorkOrderType,
                Priority = body.Priority,
                WorkOrderStatus = MaintenanceWorkOrderStatus.Open,
                Description = body.Description,
                ReasonCode = body.ReasonCode,
                AssignedRole = body.AssignedRole,
                ReportedAt = DateTimeOffset.UtcNow,
                ReportedBy = currentUser.UserId ?? Guid.Empty,
                Status = EntityStatus.Active,
            };

            if (body.WorkOrderType is MaintenanceWorkOrderType.Corrective or MaintenanceWorkOrderType.MoldChange)
                machine.OperationalStatus = MachineOperationalStatus.Down;
            else if (body.WorkOrderType == MaintenanceWorkOrderType.Preventive)
                machine.OperationalStatus = MachineOperationalStatus.PmDue;

            db.MaintenanceWorkOrders.Add(wo);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/engineering/work-orders/{wo.Id}", new { wo.Id, wo.WorkOrderNo });
        });

        g.MapGet("faults", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.MachineFaults.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.ReportedAt)
                .Select(x => new
                {
                    x.Id,
                    x.MachineId,
                    x.MaintenanceWorkOrderId,
                    x.FaultCode,
                    x.Description,
                    x.ReportedBy,
                    x.ReportedAt,
                    x.Resolved,
                    x.ResolvedAt,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("pm-schedules", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.PmSchedules.AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.Status == EntityStatus.Active)
                .OrderBy(x => x.NextDueAt ?? DateTimeOffset.MaxValue)
                .Select(x => new
                {
                    x.Id,
                    x.MachineId,
                    x.Name,
                    x.Description,
                    x.IntervalHours,
                    x.IntervalDays,
                    x.LastDoneAt,
                    x.NextDueAt,
                    x.OwnerRole,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("summary", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;
            var now = DateTimeOffset.UtcNow;

            var totalMachines = await db.Machines.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.Status == EntityStatus.Active, ct);
            var runningMachines = await db.Machines.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.Status == EntityStatus.Active && x.OperationalStatus == MachineOperationalStatus.Running, ct);
            var downMachines = await db.Machines.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.Status == EntityStatus.Active && x.OperationalStatus == MachineOperationalStatus.Down, ct);
            var pmDueMachines = await db.Machines.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.Status == EntityStatus.Active && x.OperationalStatus == MachineOperationalStatus.PmDue, ct);
            var openWorkOrders = await db.MaintenanceWorkOrders.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.WorkOrderStatus == MaintenanceWorkOrderStatus.Open, ct);
            var inProgressWorkOrders = await db.MaintenanceWorkOrders.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.WorkOrderStatus == MaintenanceWorkOrderStatus.InProgress, ct);
            var unresolvedFaults = await db.MachineFaults.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && !x.Resolved, ct);
            var overdueSchedules = await db.PmSchedules.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.Status == EntityStatus.Active && x.NextDueAt != null && x.NextDueAt < now, ct);

            return Results.Ok(new
            {
                totalMachines,
                runningMachines,
                downMachines,
                pmDueMachines,
                openWorkOrders,
                inProgressWorkOrders,
                unresolvedFaults,
                overdueSchedules,
            });
        });
    }
}
