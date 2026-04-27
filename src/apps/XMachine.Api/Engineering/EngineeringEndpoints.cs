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

public record CreatePmScheduleRequest(
    Guid MachineId,
    string Name,
    string? Description,
    int? IntervalHours,
    int? IntervalDays,
    DateTimeOffset? FirstDueAt,
    string? OwnerRole);

public record UpdateWorkOrderStatusRequest(string NewStatus, string? ClosingNotes);

public record UpdateMachineStatusRequest(string OperationalStatus);

public record CreateFaultRequest(
    Guid MachineId,
    Guid? MaintenanceWorkOrderId,
    string FaultCode,
    string Description);

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

        g.MapPost("pm-schedules", async (CreatePmScheduleRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            if (body.IntervalHours is null && body.IntervalDays is null)
                return Results.BadRequest("IntervalHours and IntervalDays cannot both be null.");

            if (string.IsNullOrWhiteSpace(body.Name))
                return Results.BadRequest("Name is required.");

            var machine = await db.Machines.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == body.MachineId && m.TenantId == tenantId && m.Status == EntityStatus.Active, ct);
            if (machine is null)
                return Results.NotFound();

            DateTimeOffset nextDueAt;
            if (body.FirstDueAt is not null)
                nextDueAt = body.FirstDueAt.Value;
            else
            {
                var delta = body.IntervalDays is not null
                    ? TimeSpan.FromDays(body.IntervalDays.Value)
                    : TimeSpan.FromHours(body.IntervalHours!.Value);
                nextDueAt = DateTimeOffset.UtcNow + delta;
            }

            var schedule = new PreventiveMaintenanceSchedule
            {
                TenantId = tenantId,
                MachineId = body.MachineId,
                Name = body.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(body.Description) ? null : body.Description.Trim(),
                IntervalHours = body.IntervalHours,
                IntervalDays = body.IntervalDays,
                NextDueAt = nextDueAt,
                OwnerRole = string.IsNullOrWhiteSpace(body.OwnerRole) ? null : body.OwnerRole.Trim(),
                Status = EntityStatus.Active,
            };

            db.PmSchedules.Add(schedule);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/engineering/pm-schedules/{schedule.Id}", new { schedule.Id, schedule.Name });
        });

        g.MapPut("work-orders/{id:guid}/status", async (Guid id, UpdateWorkOrderStatusRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            if (string.IsNullOrWhiteSpace(body.NewStatus) ||
                !Enum.TryParse<MaintenanceWorkOrderStatus>(body.NewStatus, ignoreCase: true, out var newStatus))
                return Results.BadRequest("Invalid work order status.");

            var wo = await db.MaintenanceWorkOrders
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, ct);
            if (wo is null)
                return Results.NotFound();

            if (newStatus == MaintenanceWorkOrderStatus.InProgress && wo.StartedAt is null)
                wo.StartedAt = DateTimeOffset.UtcNow;

            if (newStatus == MaintenanceWorkOrderStatus.Done)
            {
                wo.CompletedAt = DateTimeOffset.UtcNow;
                wo.ClosingNotes = body.ClosingNotes;
            }

            wo.WorkOrderStatus = newStatus;
            wo.UpdatedAt = DateTimeOffset.UtcNow;

            if (newStatus is MaintenanceWorkOrderStatus.Done or MaintenanceWorkOrderStatus.Cancelled)
            {
                var machine = await db.Machines
                    .FirstOrDefaultAsync(m => m.Id == wo.MachineId && m.TenantId == tenantId, ct);
                if (machine is not null)
                {
                    machine.OperationalStatus = MachineOperationalStatus.Idle;
                    machine.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            await db.SaveChangesAsync(ct);

            return Results.Ok(new { wo.Id, wo.WorkOrderNo, WorkOrderStatus = wo.WorkOrderStatus.ToString() });
        });

        g.MapPut("machine-status/{id:guid}", async (Guid id, UpdateMachineStatusRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            if (string.IsNullOrWhiteSpace(body.OperationalStatus) ||
                !Enum.TryParse<MachineOperationalStatus>(body.OperationalStatus, ignoreCase: true, out var opStatus))
                return Results.BadRequest("Invalid operational status.");

            var machine = await db.Machines
                .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId && m.Status == EntityStatus.Active, ct);
            if (machine is null)
                return Results.NotFound();

            machine.OperationalStatus = opStatus;
            machine.UpdatedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);

            return Results.Ok(new { machine.Id, machine.Code, OperationalStatus = machine.OperationalStatus.ToString() });
        });

        g.MapPost("faults", async (CreateFaultRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var machine = await db.Machines.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == body.MachineId && m.TenantId == tenantId && m.Status == EntityStatus.Active, ct);
            if (machine is null)
                return Results.NotFound();

            if (string.IsNullOrWhiteSpace(body.FaultCode) || string.IsNullOrWhiteSpace(body.Description))
                return Results.BadRequest("FaultCode and Description are required.");

            if (body.MaintenanceWorkOrderId is { } woId)
            {
                var linkedWo = await db.MaintenanceWorkOrders.AsNoTracking()
                    .AnyAsync(x => x.Id == woId && x.TenantId == tenantId, ct);
                if (!linkedWo)
                    return Results.NotFound();
            }

            var fault = new MachineFault
            {
                TenantId = tenantId,
                MachineId = body.MachineId,
                MaintenanceWorkOrderId = body.MaintenanceWorkOrderId,
                FaultCode = body.FaultCode.Trim(),
                Description = body.Description.Trim(),
                ReportedBy = currentUser.UserId ?? Guid.Empty,
                ReportedAt = DateTimeOffset.UtcNow,
                Resolved = false,
                Status = EntityStatus.Active,
            };

            db.MachineFaults.Add(fault);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/engineering/faults/{fault.Id}", new { fault.Id, fault.FaultCode });
        });
    }
}
