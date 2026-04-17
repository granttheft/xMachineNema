using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Quality.Domain;

namespace XMachine.Persistence.Operational.Quality.Configurations;

internal sealed class QualityMeasurementConfiguration : IEntityTypeConfiguration<QualityMeasurement>
{
    public void Configure(EntityTypeBuilder<QualityMeasurement> builder)
    {
        builder.ToTable("quality_measurements", "quality");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.QualityCheckId).HasColumnName("quality_check_id").IsRequired();

        builder.Property(x => x.ParameterCode).HasColumnName("parameter_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.MeasuredValue).HasColumnName("measured_value").HasMaxLength(512);
        builder.Property(x => x.TargetValue).HasColumnName("target_value").HasMaxLength(128);
        builder.Property(x => x.MinValue).HasColumnName("min_value").HasMaxLength(128);
        builder.Property(x => x.MaxValue).HasColumnName("max_value").HasMaxLength(128);
        builder.Property(x => x.Result).HasColumnName("result").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => x.QualityCheckId);

        builder.HasOne<QualityCheck>()
            .WithMany()
            .HasForeignKey(x => x.QualityCheckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
