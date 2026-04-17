using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Eventing.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Eventing.Configurations;

internal sealed class OeeSnapshotConfiguration : IEntityTypeConfiguration<OeeSnapshot>
{
    public void Configure(EntityTypeBuilder<OeeSnapshot> builder)
    {
        builder.ToTable("oee_snapshots", "eventing");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.SiteId).HasColumnName("site_id");
        builder.Property(x => x.LineId).HasColumnName("line_id");
        builder.Property(x => x.MachineId).HasColumnName("machine_id");

        builder.Property(x => x.PeriodType).HasColumnName("period_type").IsRequired();
        builder.Property(x => x.PeriodStart).HasColumnName("period_start").IsRequired();
        builder.Property(x => x.PeriodEnd).HasColumnName("period_end").IsRequired();

        builder.Property(x => x.Availability).HasColumnName("availability").HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.Performance).HasColumnName("performance").HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.Quality).HasColumnName("quality").HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.OeeValue).HasColumnName("oee_value").HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.PeriodStart, x.PeriodType });

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
    }
}
