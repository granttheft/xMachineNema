using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Engineering.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Engineering.Configurations;

internal sealed class MachineFaultConfiguration : IEntityTypeConfiguration<MachineFault>
{
    public void Configure(EntityTypeBuilder<MachineFault> builder)
    {
        builder.ToTable("machine_faults", "engineering");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id").IsRequired();
        builder.Property(x => x.MaintenanceWorkOrderId).HasColumnName("maintenance_work_order_id");

        builder.Property(x => x.FaultCode).HasColumnName("fault_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(2048).IsRequired();

        builder.Property(x => x.ReportedBy).HasColumnName("reported_by").IsRequired();
        builder.Property(x => x.ReportedAt).HasColumnName("reported_at").IsRequired();

        builder.Property(x => x.Resolved).HasColumnName("resolved").IsRequired();
        builder.Property(x => x.ResolvedAt).HasColumnName("resolved_at");

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.MachineId });
        builder.HasIndex(x => new { x.TenantId, x.ReportedAt });

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MaintenanceWorkOrder>()
            .WithMany()
            .HasForeignKey(x => x.MaintenanceWorkOrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
