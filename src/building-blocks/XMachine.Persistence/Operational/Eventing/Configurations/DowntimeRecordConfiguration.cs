using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Eventing.Domain;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Eventing.Configurations;

internal sealed class DowntimeRecordConfiguration : IEntityTypeConfiguration<DowntimeRecord>
{
    public void Configure(EntityTypeBuilder<DowntimeRecord> builder)
    {
        builder.ToTable("downtime_records", "eventing");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id");
        builder.Property(x => x.LineId).HasColumnName("line_id");
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id");

        builder.Property(x => x.DowntimeReasonCode).HasColumnName("downtime_reason_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.DowntimeReasonText).HasColumnName("downtime_reason_text").HasMaxLength(1024);
        builder.Property(x => x.PlannedFlag).HasColumnName("planned_flag").IsRequired();

        builder.Property(x => x.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(x => x.EndTime).HasColumnName("end_time");
        builder.Property(x => x.DurationMs).HasColumnName("duration_ms");

        builder.Property(x => x.EnteredBy).HasColumnName("entered_by");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.StartTime });

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Line>()
            .WithMany()
            .HasForeignKey(x => x.LineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.EnteredBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
