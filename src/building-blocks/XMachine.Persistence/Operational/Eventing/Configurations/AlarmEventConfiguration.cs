using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Eventing.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Eventing.Configurations;

internal sealed class AlarmEventConfiguration : IEntityTypeConfiguration<AlarmEvent>
{
    public void Configure(EntityTypeBuilder<AlarmEvent> builder)
    {
        builder.ToTable("alarm_events", "eventing");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.SiteId).HasColumnName("site_id");
        builder.Property(x => x.LineId).HasColumnName("line_id");
        builder.Property(x => x.MachineId).HasColumnName("machine_id");

        builder.Property(x => x.AlarmCode).HasColumnName("alarm_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AlarmText).HasColumnName("alarm_text").HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Severity).HasColumnName("severity").IsRequired();
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(128).IsRequired();

        builder.Property(x => x.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(x => x.EndTime).HasColumnName("end_time");
        builder.Property(x => x.DurationMs).HasColumnName("duration_ms");

        builder.Property(x => x.AckBy).HasColumnName("ack_by");
        builder.Property(x => x.AckTime).HasColumnName("ack_time");

        builder.Property(x => x.AlarmStatus).HasColumnName("alarm_status").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.StartTime });

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => x.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Line>()
            .WithMany()
            .HasForeignKey(x => x.LineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.AckBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
