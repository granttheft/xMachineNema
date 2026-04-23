using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Engineering.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Engineering.Configurations;

internal sealed class MaintenanceWorkOrderConfiguration : IEntityTypeConfiguration<MaintenanceWorkOrder>
{
    public void Configure(EntityTypeBuilder<MaintenanceWorkOrder> builder)
    {
        builder.ToTable("maintenance_work_orders", "engineering");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id").IsRequired();
        builder.Property(x => x.LineId).HasColumnName("line_id");

        builder.Property(x => x.WorkOrderNo).HasColumnName("work_order_no").HasMaxLength(64).IsRequired();
        builder.Property(x => x.WorkOrderType).HasColumnName("work_order_type").IsRequired();
        builder.Property(x => x.Priority).HasColumnName("priority").IsRequired();
        builder.Property(x => x.WorkOrderStatus).HasColumnName("work_order_status").IsRequired();

        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(2048).IsRequired();
        builder.Property(x => x.ReasonCode).HasColumnName("reason_code").HasMaxLength(64);

        builder.Property(x => x.AssignedTo).HasColumnName("assigned_to");
        builder.Property(x => x.AssignedRole).HasColumnName("assigned_role").HasMaxLength(64);

        builder.Property(x => x.ReportedAt).HasColumnName("reported_at").IsRequired();
        builder.Property(x => x.ReportedBy).HasColumnName("reported_by").IsRequired();
        builder.Property(x => x.StartedAt).HasColumnName("started_at");
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at");
        builder.Property(x => x.ClosingNotes).HasColumnName("closing_notes").HasMaxLength(2048);

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.WorkOrderNo }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.MachineId });
        builder.HasIndex(x => new { x.TenantId, x.WorkOrderStatus });

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
