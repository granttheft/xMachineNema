using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class MaterialConsumptionConfiguration : IEntityTypeConfiguration<MaterialConsumption>
{
    public void Configure(EntityTypeBuilder<MaterialConsumption> builder)
    {
        builder.ToTable("material_consumptions", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.LotBatchId).HasColumnName("lot_batch_id").IsRequired();
        builder.Property(x => x.MaterialCode).HasColumnName("material_code").HasMaxLength(128).IsRequired();
        builder.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(256);
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(16).IsRequired();
        builder.Property(x => x.ConsumedAt).HasColumnName("consumed_at").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => x.LotBatchId);

        builder.HasOne<LotBatch>()
            .WithMany()
            .HasForeignKey(x => x.LotBatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
