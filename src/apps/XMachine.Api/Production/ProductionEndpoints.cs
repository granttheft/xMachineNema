using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Module.Platform.Domain;
using XMachine.Module.Production.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Api.Production;

public record CreateJobRequest(
    Guid MachineId,
    Guid LineId,
    Guid ProductionOrderId,
    Guid? RecipeId,
    Guid? ShiftId,
    int PlannedQty,
    DateTimeOffset? PlannedStartAt,
    DateTimeOffset? PlannedEndAt,
    string? Notes);

public record UpdateJobStatusRequest(string NewStatus, string? PauseReason, string? PauseNotes);

public record CreateDeclarationRequest(
    Guid JobExecutionId,
    Guid MachineId,
    int DeclaredQty,
    int ScrapQty,
    int DefectQty,
    string? Notes);

public record UpdateMachineJobRequest(Guid? JobExecutionId, Guid? OperatorId);

public static class ProductionEndpoints
{
    /// <summary>Maps production read/write endpoints under <c>/api/production</c>.</summary>
    public static void MapProductionEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/production").RequireAuthorization();

        g.MapGet("jobs", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.JobExecutions.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.ActualStartAt == null ? 1 : 0)
                .ThenByDescending(x => x.ActualStartAt)
                .ThenByDescending(x => x.PlannedStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.MachineId,
                    x.LineId,
                    x.ProductionOrderId,
                    x.RecipeId,
                    x.ShiftId,
                    x.OperatorId,
                    x.JobNo,
                    x.ExecutionStatus,
                    x.PlannedQty,
                    x.ProducedQty,
                    x.ScrapQty,
                    x.DefectQty,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.ActualStartAt,
                    x.ActualEndAt,
                    x.PausedAt,
                    x.PauseReason,
                    x.PauseNotes,
                    x.Notes,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("jobs/active", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var rows = await db.JobExecutions.AsNoTracking()
                .Where(x => x.TenantId == tenantId &&
                            (x.ExecutionStatus == JobExecutionStatus.Running ||
                             x.ExecutionStatus == JobExecutionStatus.Setup ||
                             x.ExecutionStatus == JobExecutionStatus.Paused))
                .OrderByDescending(x => x.ActualStartAt)
                .ThenByDescending(x => x.PlannedStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.MachineId,
                    x.LineId,
                    x.ProductionOrderId,
                    x.RecipeId,
                    x.ShiftId,
                    x.OperatorId,
                    x.JobNo,
                    x.ExecutionStatus,
                    x.PlannedQty,
                    x.ProducedQty,
                    x.ScrapQty,
                    x.DefectQty,
                    x.PlannedStartAt,
                    x.PlannedEndAt,
                    x.ActualStartAt,
                    x.ActualEndAt,
                    x.PausedAt,
                    x.PauseReason,
                    x.PauseNotes,
                    x.Notes,
                    x.Status,
                })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        g.MapGet("machines", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var machines = await db.Machines.AsNoTracking()
                .Where(m => m.TenantId == tenantId && m.Status == EntityStatus.Active)
                .OrderBy(m => m.Code)
                .Select(m => new
                {
                    m.Id,
                    m.Code,
                    m.Name,
                    m.LineId,
                    m.OperationalStatus,
                })
                .ToListAsync(ct);

            var activeJobs = await db.JobExecutions.AsNoTracking()
                .Where(j => j.TenantId == tenantId &&
                            (j.ExecutionStatus == JobExecutionStatus.Running ||
                             j.ExecutionStatus == JobExecutionStatus.Setup ||
                             j.ExecutionStatus == JobExecutionStatus.Paused))
                .ToListAsync(ct);

            var latestByMachine = activeJobs
                .GroupBy(j => j.MachineId)
                .ToDictionary(
                    gr => gr.Key,
                    gr => gr.OrderByDescending(x => x.ActualStartAt ?? x.PlannedStartAt ?? DateTimeOffset.MinValue).First());

            var result = machines.Select(m =>
            {
                latestByMachine.TryGetValue(m.Id, out var job);
                return new
                {
                    m.Id,
                    m.Code,
                    m.Name,
                    m.LineId,
                    m.OperationalStatus,
                    activeJob = job is null
                        ? null
                        : new
                        {
                            job.Id,
                            job.JobNo,
                            job.ExecutionStatus,
                            job.ProducedQty,
                            job.PlannedQty,
                            job.ScrapQty,
                            job.OperatorId,
                            job.ActualStartAt,
                            job.PauseReason,
                        },
                };
            }).ToList();

            return Results.Ok(result);
        });

        g.MapGet("summary", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;
            var todayStart = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);

            var totalMachines = await db.Machines.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.Status == EntityStatus.Active, ct);

            var runningJobs = await db.JobExecutions.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.ExecutionStatus == JobExecutionStatus.Running, ct);

            var pausedJobs = await db.JobExecutions.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.ExecutionStatus == JobExecutionStatus.Paused, ct);

            var queuedJobs = await db.JobExecutions.AsNoTracking()
                .CountAsync(x => x.TenantId == tenantId && x.ExecutionStatus == JobExecutionStatus.Queued, ct);

            var completedToday = await db.JobExecutions.AsNoTracking()
                .CountAsync(x =>
                    x.TenantId == tenantId &&
                    x.ExecutionStatus == JobExecutionStatus.Done &&
                    x.ActualEndAt != null &&
                    x.ActualEndAt >= todayStart, ct);

            var totalProducedToday = await db.JobExecutions.AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ExecutionStatus == JobExecutionStatus.Done &&
                    x.ActualEndAt != null &&
                    x.ActualEndAt >= todayStart)
                .SumAsync(x => (int?)x.ProducedQty, ct) ?? 0;

            var totalScrapToday = await db.JobExecutions.AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ExecutionStatus == JobExecutionStatus.Done &&
                    x.ActualEndAt != null &&
                    x.ActualEndAt >= todayStart)
                .SumAsync(x => (int?)x.ScrapQty, ct) ?? 0;

            return Results.Ok(new
            {
                totalMachines,
                runningJobs,
                pausedJobs,
                queuedJobs,
                completedToday,
                totalProducedToday,
                totalScrapToday,
            });
        });

        g.MapPost("jobs", async (CreateJobRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var machine = await db.Machines
                .FirstOrDefaultAsync(m => m.Id == body.MachineId && m.TenantId == tenantId && m.Status == EntityStatus.Active, ct);
            if (machine is null)
                return Results.NotFound();

            if (machine.LineId != body.LineId)
                return Results.BadRequest("LineId does not match the machine line.");

            var order = await db.ProductionOrders
                .FirstOrDefaultAsync(o => o.Id == body.ProductionOrderId && o.TenantId == tenantId && o.Status == EntityStatus.Active, ct);
            if (order is null)
                return Results.NotFound();

            if (order.LineId is not null && order.LineId != body.LineId)
                return Results.BadRequest("Production order line does not match LineId.");

            var dayPrefix = $"JOB-{DateTime.UtcNow:yyyyMMdd}-";
            var countToday = await db.JobExecutions
                .CountAsync(j => j.TenantId == tenantId && j.JobNo.StartsWith(dayPrefix), ct);
            var jobNo = $"{dayPrefix}{countToday + 1:000}";

            var job = new JobExecution
            {
                TenantId = tenantId,
                MachineId = body.MachineId,
                LineId = body.LineId,
                ProductionOrderId = body.ProductionOrderId,
                RecipeId = body.RecipeId,
                ShiftId = body.ShiftId,
                OperatorId = null,
                JobNo = jobNo,
                ExecutionStatus = JobExecutionStatus.Queued,
                PlannedQty = body.PlannedQty,
                ProducedQty = 0,
                ScrapQty = 0,
                DefectQty = 0,
                PlannedStartAt = body.PlannedStartAt,
                PlannedEndAt = body.PlannedEndAt,
                Notes = body.Notes,
                Status = EntityStatus.Active,
            };

            db.JobExecutions.Add(job);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/production/jobs/{job.Id}", new { job.Id, job.JobNo });
        });

        g.MapPut("jobs/{id:guid}/status", async (Guid id, UpdateJobStatusRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            if (!Enum.TryParse<JobExecutionStatus>(body.NewStatus, ignoreCase: true, out var newStatus))
                return Results.BadRequest($"Invalid status: {body.NewStatus}");

            var job = await db.JobExecutions
                .FirstOrDefaultAsync(j => j.Id == id && j.TenantId == tenantId, ct);
            if (job is null)
                return Results.NotFound();

            var machine = await db.Machines
                .FirstOrDefaultAsync(m => m.Id == job.MachineId && m.TenantId == tenantId, ct);

            var transitionError = ApplyJobStatusTransition(job, machine, newStatus, body.PauseReason, body.PauseNotes);
            if (transitionError is not null)
                return Results.BadRequest(transitionError);

            await db.SaveChangesAsync(ct);

            return Results.Ok(new { job.Id, job.JobNo, ExecutionStatus = job.ExecutionStatus.ToString() });
        });

        g.MapPost("declarations", async (CreateDeclarationRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var tenantId = currentUser.TenantId.Value;

            var job = await db.JobExecutions
                .FirstOrDefaultAsync(j => j.Id == body.JobExecutionId && j.TenantId == tenantId, ct);
            if (job is null)
                return Results.NotFound();

            if (job.ExecutionStatus != JobExecutionStatus.Running && job.ExecutionStatus != JobExecutionStatus.Paused)
                return Results.BadRequest("Job must be Running or Paused");

            if (job.MachineId != body.MachineId)
                return Results.BadRequest("MachineId does not match the job machine.");

            var decl = new OperatorDeclaration
            {
                TenantId = tenantId,
                JobExecutionId = body.JobExecutionId,
                MachineId = body.MachineId,
                OperatorId = currentUser.UserId.Value,
                DeclaredQty = body.DeclaredQty,
                ScrapQty = body.ScrapQty,
                DefectQty = body.DefectQty,
                Notes = body.Notes,
                DeclaredAt = DateTimeOffset.UtcNow,
                Status = EntityStatus.Active,
            };

            job.ProducedQty += body.DeclaredQty;
            job.ScrapQty += body.ScrapQty;
            job.DefectQty += body.DefectQty;

            db.OperatorDeclarations.Add(decl);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/production/declarations/{decl.Id}", new
            {
                decl.Id,
                decl.DeclaredAt,
                newProducedQty = job.ProducedQty,
            });
        });

        g.MapPut("machines/{id:guid}/job", async (Guid id, UpdateMachineJobRequest body, XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null) return Results.Unauthorized();
            var tenantId = currentUser.TenantId.Value;

            var machine = await db.Machines
                .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId && m.Status == EntityStatus.Active, ct);
            if (machine is null)
                return Results.NotFound();

            if (body.JobExecutionId is null)
            {
                machine.OperationalStatus = MachineOperationalStatus.Idle;
                machine.UpdatedAt = DateTimeOffset.UtcNow;

                var activeOnMachine = await db.JobExecutions
                    .Where(j =>
                        j.TenantId == tenantId &&
                        j.MachineId == machine.Id &&
                        (j.ExecutionStatus == JobExecutionStatus.Running ||
                         j.ExecutionStatus == JobExecutionStatus.Setup ||
                         j.ExecutionStatus == JobExecutionStatus.Paused))
                    .ToListAsync(ct);

                foreach (var j in activeOnMachine)
                {
                    if (body.OperatorId is null || j.OperatorId == body.OperatorId)
                        j.OperatorId = null;
                }
            }
            else
            {
                var execJob = await db.JobExecutions
                    .FirstOrDefaultAsync(j => j.Id == body.JobExecutionId.Value && j.TenantId == tenantId, ct);
                if (execJob is null)
                    return Results.NotFound();

                if (execJob.MachineId != machine.Id)
                    return Results.BadRequest("Job is not assigned to this machine.");

                if (execJob.ExecutionStatus is not JobExecutionStatus.Queued
                    and not JobExecutionStatus.Setup
                    and not JobExecutionStatus.Running)
                    return Results.BadRequest("Job execution status must be Queued, Setup, or Running.");

                execJob.OperatorId = body.OperatorId;
                machine.UpdatedAt = DateTimeOffset.UtcNow;
                machine.OperationalStatus = execJob.ExecutionStatus == JobExecutionStatus.Running
                    ? MachineOperationalStatus.Running
                    : machine.OperationalStatus;
            }

            await db.SaveChangesAsync(ct);

            return Results.Ok(new { machineId = machine.Id, operatorId = body.OperatorId });
        });
    }

    private static string? ApplyJobStatusTransition(
        JobExecution job,
        Machine? machine,
        JobExecutionStatus newStatus,
        string? pauseReasonRaw,
        string? pauseNotes)
    {
        var current = job.ExecutionStatus;

        if (current is JobExecutionStatus.Done or JobExecutionStatus.Cancelled)
            return "Job is already in a terminal state.";

        if (newStatus == JobExecutionStatus.Cancelled)
        {
            job.ExecutionStatus = JobExecutionStatus.Cancelled;
            job.ActualEndAt = DateTimeOffset.UtcNow;
            if (machine is not null)
            {
                machine.OperationalStatus = MachineOperationalStatus.Idle;
                machine.UpdatedAt = DateTimeOffset.UtcNow;
            }

            return null;
        }

        return (current, newStatus) switch
        {
            (JobExecutionStatus.Queued, JobExecutionStatus.Setup) => ApplyQueuedToSetup(job),
            (JobExecutionStatus.Setup, JobExecutionStatus.Running) => ApplySetupToRunning(job),
            (JobExecutionStatus.Running, JobExecutionStatus.Paused) => ApplyRunningToPaused(job, pauseReasonRaw, pauseNotes),
            (JobExecutionStatus.Paused, JobExecutionStatus.Running) => ApplyPausedToRunning(job),
            (JobExecutionStatus.Running, JobExecutionStatus.Done) => ApplyToDone(job, machine),
            (JobExecutionStatus.Paused, JobExecutionStatus.Done) => ApplyToDone(job, machine),
            _ => $"Invalid transition from {current} to {newStatus}.",
        };
    }

    private static string? ApplyQueuedToSetup(JobExecution job)
    {
        job.ExecutionStatus = JobExecutionStatus.Setup;
        job.ActualStartAt = DateTimeOffset.UtcNow;
        return null;
    }

    private static string? ApplySetupToRunning(JobExecution job)
    {
        job.ExecutionStatus = JobExecutionStatus.Running;
        job.ActualStartAt ??= DateTimeOffset.UtcNow;
        return null;
    }

    private static string? ApplyRunningToPaused(JobExecution job, string? pauseReasonRaw, string? pauseNotes)
    {
        job.ExecutionStatus = JobExecutionStatus.Paused;
        job.PausedAt = DateTimeOffset.UtcNow;
        ResolvePauseFields(pauseReasonRaw, pauseNotes, out var reason, out var notes);
        job.PauseReason = reason;
        job.PauseNotes = notes;
        return null;
    }

    private static string? ApplyPausedToRunning(JobExecution job)
    {
        job.ExecutionStatus = JobExecutionStatus.Running;
        job.PausedAt = null;
        job.PauseReason = null;
        job.PauseNotes = null;
        return null;
    }

    private static string? ApplyToDone(JobExecution job, Machine? machine)
    {
        job.ExecutionStatus = JobExecutionStatus.Done;
        job.ActualEndAt = DateTimeOffset.UtcNow;
        if (machine is not null)
        {
            machine.OperationalStatus = MachineOperationalStatus.Idle;
            machine.UpdatedAt = DateTimeOffset.UtcNow;
        }

        return null;
    }

    private static void ResolvePauseFields(
        string? pauseReasonRaw,
        string? pauseNotesIn,
        out JobPauseReason? pauseReason,
        out string? pauseNotes)
    {
        if (string.IsNullOrWhiteSpace(pauseReasonRaw))
        {
            pauseReason = null;
            pauseNotes = pauseNotesIn;
            return;
        }

        if (Enum.TryParse<JobPauseReason>(pauseReasonRaw.Trim(), ignoreCase: true, out var parsed))
        {
            pauseReason = parsed;
            pauseNotes = pauseNotesIn;
            return;
        }

        pauseReason = JobPauseReason.Other;
        pauseNotes = string.IsNullOrWhiteSpace(pauseNotesIn)
            ? pauseReasonRaw.Trim()
            : $"{pauseReasonRaw.Trim()} — {pauseNotesIn}";
    }
}
