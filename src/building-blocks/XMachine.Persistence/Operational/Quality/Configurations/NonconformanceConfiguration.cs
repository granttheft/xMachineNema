using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Quality.Domain;

namespace XMachine.Persistence.Operational.Quality.Configurations;

internal sealed class NonconformanceConfiguration : IEntityTypeConfiguration<Nonconformance>
{
    public void Configure(EntityTypeBuilder<Nonconformance> builder)
    {
        builder.ToTable("nonconformances", "quality");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.QualityCheckId).HasColumnName("quality_check_id").IsRequired();

        builder.Property(x => x.NcCode).HasColumnName("nc_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Severity).HasColumnName("severity").IsRequired();
        builder.Property(x => x.NcStatus).HasColumnName("nc_status").IsRequired();
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
