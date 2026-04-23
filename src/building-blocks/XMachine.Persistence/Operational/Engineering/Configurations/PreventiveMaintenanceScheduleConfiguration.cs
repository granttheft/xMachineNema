using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Engineering.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Engineering.Configurations;

internal sealed class PreventiveMaintenanceScheduleConfiguration : IEntityTypeConfiguration<PreventiveMaintenanceSchedule>
{
    public void Configure(EntityTypeBuilder<PreventiveMaintenanceSchedule> builder)
    {
        builder.ToTable("pm_schedules", "engineering");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id").IsRequired();

        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(2048);

        builder.Property(x => x.IntervalHours).HasColumnName("interval_hours");
        builder.Property(x => x.IntervalDays).HasColumnName("interval_days");

        builder.Property(x => x.LastDoneAt).HasColumnName("last_done_at");
        builder.Property(x => x.NextDueAt).HasColumnName("next_due_at");

        builder.Property(x => x.OwnerRole).HasColumnName("owner_role").HasMaxLength(64);

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.MachineId });
        builder.HasIndex(x => new { x.TenantId, x.NextDueAt });

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
