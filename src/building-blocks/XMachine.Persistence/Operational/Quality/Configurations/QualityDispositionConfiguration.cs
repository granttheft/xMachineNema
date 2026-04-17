using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Quality.Domain;

namespace XMachine.Persistence.Operational.Quality.Configurations;

internal sealed class QualityDispositionConfiguration : IEntityTypeConfiguration<QualityDisposition>
{
    public void Configure(EntityTypeBuilder<QualityDisposition> builder)
    {
        builder.ToTable("quality_dispositions", "quality");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.NonconformanceId).HasColumnName("nonconformance_id").IsRequired();

        builder.Property(x => x.DispositionType).HasColumnName("disposition_type").IsRequired();
        builder.Property(x => x.DecisionTime).HasColumnName("decision_time").IsRequired();
        builder.Property(x => x.DecidedBy).HasColumnName("decided_by");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => x.NonconformanceId);

        builder.HasOne<Nonconformance>()
            .WithMany()
            .HasForeignKey(x => x.NonconformanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.DecidedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
