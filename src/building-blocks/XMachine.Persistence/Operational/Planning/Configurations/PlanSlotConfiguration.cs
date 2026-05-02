using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Planning.Domain;

namespace XMachine.Persistence.Operational.Planning.Configurations;

internal sealed class PlanSlotConfiguration : IEntityTypeConfiguration<PlanSlot>
{
    public void Configure(EntityTypeBuilder<PlanSlot> builder)
    {
        builder.ToTable("plan_slots", "planning");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();

        builder.Property(x => x.ProductionPlanId).HasColumnName("production_plan_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id").IsRequired();
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id");
        builder.Property(x => x.JobExecutionId).HasColumnName("job_execution_id");

        builder.Property(x => x.SlotNo).HasColumnName("slot_no").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SlotStatus).HasColumnName("slot_status").IsRequired();
        builder.Property(x => x.Priority).HasColumnName("priority").IsRequired();

        builder.Property(x => x.PlannedQty).HasColumnName("planned_qty").IsRequired();
        builder.Property(x => x.PlannedStartAt).HasColumnName("planned_start_at").IsRequired();
        builder.Property(x => x.PlannedEndAt).HasColumnName("planned_end_at").IsRequired();

        builder.Property(x => x.EstimatedDurationMinutes).HasColumnName("estimated_duration_minutes");
        builder.Property(x => x.SetupTimeMinutes).HasColumnName("setup_time_minutes");
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(4000);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.ProductionPlanId });
        builder.HasIndex(x => new { x.TenantId, x.MachineId });
        builder.HasIndex(x => new { x.TenantId, x.SlotStatus });
        builder.HasIndex(x => new { x.TenantId, x.PlannedStartAt });

        builder.HasOne<ProductionPlan>()
            .WithMany()
            .HasForeignKey(x => x.ProductionPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
