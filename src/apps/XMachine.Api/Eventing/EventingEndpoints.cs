using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using XMachine.Api.Hubs;
using XMachine.Module.Auth.Security;
using XMachine.Module.Eventing.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Api.Eventing;

/// <summary>Request body for creating an ongoing downtime record.</summary>
public sealed record CreateDowntimeRequest(
    Guid? MachineId,
    Guid? LineId,
    Guid? ProductionOrderId,
    string DowntimeReasonCode,
    string? DowntimeReasonText,
    bool PlannedFlag);

/// <summary>
/// Request body for acknowledging an alarm.
/// <see cref="AckNote"/> is optional and is not persisted on the entity.
/// </summary>
public sealed record AcknowledgeAlarmRequest(string? AckNote);

public static class EventingEndpoints
{
    public static void MapEventingEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/eventing").RequireAuthorization();

        g.MapGet("alarms", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.AlarmEvents.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.StartTime)
                .Select(x => new
                {
                    x.Id,
                    x.AlarmCode,
                    x.AlarmText,
                    x.Severity,
                    x.Category,
                    x.StartTime,
                    x.EndTime,
                    x.DurationMs,
                    x.SiteId,
                    x.LineId,
                    x.MachineId,
                    x.AckBy,
                    x.AckTime,
                    x.AlarmStatus,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("downtimes", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.DowntimeRecords.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.StartTime)
                .Select(x => new
                {
                    x.Id,
                    x.DowntimeReasonCode,
                    x.DowntimeReasonText,
                    x.PlannedFlag,
                    x.StartTime,
                    x.EndTime,
                    x.DurationMs,
                    x.MachineId,
                    x.LineId,
                    x.ProductionOrderId,
                    x.EnteredBy,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapPost("downtimes", async (
            CreateDowntimeRequest body,
            XMachineDbContext db,
            ICurrentUser currentUser,
            IHubContext<XMachineHub, IXMachineHubClient> hub,
            CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            if (string.IsNullOrWhiteSpace(body.DowntimeReasonCode))
                return Results.BadRequest("ReasonCode is required");

            var now = DateTimeOffset.UtcNow;

            var record = new DowntimeRecord
            {
                TenantId = tenantId,
                MachineId = body.MachineId,
                LineId = body.LineId,
                ProductionOrderId = body.ProductionOrderId,
                DowntimeReasonCode = body.DowntimeReasonCode.Trim(),
                DowntimeReasonText = body.DowntimeReasonText,
                PlannedFlag = body.PlannedFlag,
                StartTime = now,
                EndTime = null,
                DurationMs = null,
                EnteredBy = currentUser.UserId ?? Guid.Empty,
                Status = EntityStatus.Active,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = currentUser.UserId ?? Guid.Empty,
                UpdatedBy = currentUser.UserId ?? Guid.Empty,
            };

            db.DowntimeRecords.Add(record);
            await db.SaveChangesAsync(ct);

            _ = PushDowntimeMachineStatusAsync(hub, body);

            return Results.Created(
                $"/api/eventing/downtimes/{record.Id}",
                new { record.Id, record.StartTime, record.DowntimeReasonCode });
        });

        g.MapPut("alarms/{id:guid}/ack", async (
            Guid id,
            AcknowledgeAlarmRequest ackRequest,
            XMachineDbContext db,
            ICurrentUser currentUser,
            IHubContext<XMachineHub, IXMachineHubClient> hub,
            CancellationToken ct) =>
        {
            ArgumentNullException.ThrowIfNull(ackRequest);

            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var alarm = await db.AlarmEvents
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId, ct);
            if (alarm is null)
                return Results.NotFound();

            if (alarm.AlarmStatus == AlarmLifecycleStatus.Acknowledged)
                return Results.BadRequest("Already acknowledged");

            var now = DateTimeOffset.UtcNow;
            var userId = currentUser.UserId ?? Guid.Empty;

            alarm.AckBy = currentUser.UserId ?? Guid.Empty;
            alarm.AckTime = now;
            alarm.AlarmStatus = AlarmLifecycleStatus.Acknowledged;
            alarm.UpdatedAt = now;
            alarm.UpdatedBy = currentUser.UserId ?? Guid.Empty;

            await db.SaveChangesAsync(ct);

            _ = PushAlarmAcknowledgedAsync(hub, alarm);

            return Results.Ok(new { alarm.Id, alarm.AlarmStatus, alarm.AckTime });
        });

        g.MapGet("oee", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.OeeSnapshots.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.PeriodStart)
                .Select(x => new
                {
                    x.Id,
                    x.PeriodType,
                    x.PeriodStart,
                    x.PeriodEnd,
                    x.SiteId,
                    x.LineId,
                    x.MachineId,
                    x.Availability,
                    x.Performance,
                    x.Quality,
                    x.OeeValue,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("kpis", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var defs = await db.KpiDefinitions.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.Code)
                .Select(x => new { x.Id, x.Code, x.Name, x.ScopeType, x.DataSourceType, x.Status })
                .ToListAsync(ct);
            var results = await db.KpiResults.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.PeriodStart)
                .Select(x => new
                {
                    x.Id,
                    x.KpiDefinitionId,
                    x.ScopeType,
                    x.ScopeId,
                    x.PeriodStart,
                    x.PeriodEnd,
                    x.Value,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(new { definitions = defs, results });
        });

        g.MapGet("summary", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var alarms = await db.AlarmEvents.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var downtimes = await db.DowntimeRecords.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var oee = await db.OeeSnapshots.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var kpiDefs = await db.KpiDefinitions.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            var kpiResults = await db.KpiResults.AsNoTracking().Where(x => x.TenantId == tenantId).CountAsync(ct);
            return Results.Ok(new { alarms, downtimes, oee, kpiDefs, kpiResults });
        });
    }

    /// <summary>Pushes a machine status change to all hub clients; failures are swallowed.</summary>
    private static async Task PushDowntimeMachineStatusAsync(
        IHubContext<XMachineHub, IXMachineHubClient> hub,
        CreateDowntimeRequest body)
    {
        try
        {
            await hub.Clients.All.MachineStatusChanged(new MachineStatusChangedEvent(
                body.MachineId ?? Guid.Empty,
                "—",
                "—",
                "Running",
                "Down",
                DateTimeOffset.UtcNow));
        }
        catch
        {
        }
    }

    /// <summary>Pushes an alarm update reflecting acknowledgement to all hub clients; failures are swallowed.</summary>
    private static async Task PushAlarmAcknowledgedAsync(
        IHubContext<XMachineHub, IXMachineHubClient> hub,
        AlarmEvent alarm)
    {
        try
        {
            await hub.Clients.All.AlarmTriggered(new AlarmTriggeredEvent(
                alarm.Id,
                alarm.MachineId ?? Guid.Empty,
                alarm.AlarmCode,
                "ACKNOWLEDGED: " + alarm.AlarmText,
                alarm.Severity.ToString(),
                alarm.AckTime!.Value));
        }
        catch
        {
        }
    }
}
