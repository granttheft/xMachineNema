using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Auth.Domain;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;
using XMachine.Module.Quality.Domain;

namespace XMachine.Persistence.Operational.Quality.Configurations;

internal sealed class QualityCheckConfiguration : IEntityTypeConfiguration<QualityCheck>
{
    public void Configure(EntityTypeBuilder<QualityCheck> builder)
    {
        builder.ToTable("quality_checks", "quality");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id");
        builder.Property(x => x.LotBatchId).HasColumnName("lot_batch_id");
        builder.Property(x => x.MachineId).HasColumnName("machine_id");

        builder.Property(x => x.CheckType).HasColumnName("check_type").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CheckTime).HasColumnName("check_time").IsRequired();
        builder.Property(x => x.CheckStatus).HasColumnName("check_status").IsRequired();
        builder.Property(x => x.ApprovedBy).HasColumnName("approved_by");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.CheckTime });

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<LotBatch>()
            .WithMany()
            .HasForeignKey(x => x.LotBatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
