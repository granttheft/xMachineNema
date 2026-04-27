using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;
using XMachine.Module.Production.Domain;

namespace XMachine.Persistence.Operational.Production.Configurations;

internal sealed class JobExecutionConfiguration : IEntityTypeConfiguration<JobExecution>
{
    public void Configure(EntityTypeBuilder<JobExecution> builder)
    {
        builder.ToTable("job_executions", "production");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id").IsRequired();
        builder.Property(x => x.LineId).HasColumnName("line_id").IsRequired();
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(x => x.RecipeId).HasColumnName("recipe_id");
        builder.Property(x => x.ShiftId).HasColumnName("shift_id");
        builder.Property(x => x.OperatorId).HasColumnName("operator_id");

        builder.Property(x => x.JobNo).HasColumnName("job_no").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ExecutionStatus).HasColumnName("execution_status").IsRequired();

        builder.Property(x => x.PlannedQty).HasColumnName("planned_qty").IsRequired();
        builder.Property(x => x.ProducedQty).HasColumnName("produced_qty").IsRequired();
        builder.Property(x => x.ScrapQty).HasColumnName("scrap_qty").IsRequired();
        builder.Property(x => x.DefectQty).HasColumnName("defect_qty").IsRequired();

        builder.Property(x => x.PlannedStartAt).HasColumnName("planned_start_at");
        builder.Property(x => x.PlannedEndAt).HasColumnName("planned_end_at");
        builder.Property(x => x.ActualStartAt).HasColumnName("actual_start_at");
        builder.Property(x => x.ActualEndAt).HasColumnName("actual_end_at");
        builder.Property(x => x.PausedAt).HasColumnName("paused_at");

        builder.Property(x => x.PauseReason).HasColumnName("pause_reason");
        builder.Property(x => x.PauseNotes).HasColumnName("pause_notes").HasMaxLength(2048);
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2048);

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.MachineId });
        builder.HasIndex(x => new { x.TenantId, x.ProductionOrderId });
        builder.HasIndex(x => new { x.TenantId, x.ExecutionStatus });
        builder.HasIndex(x => new { x.TenantId, x.ShiftId });
        builder.HasIndex(x => new { x.TenantId, x.JobNo }).IsUnique();

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
