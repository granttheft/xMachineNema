using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("inventory_movements", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.MovementType).HasColumnName("movement_type").IsRequired();
        builder.Property(x => x.MaterialCode).HasColumnName("material_code").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(16).IsRequired();
        builder.Property(x => x.LotBatchId).HasColumnName("lot_batch_id");
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id");
        builder.Property(x => x.ReferenceType).HasColumnName("reference_type").HasMaxLength(64);
        builder.Property(x => x.ReferenceId).HasColumnName("reference_id");
        builder.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(1024);
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.OccurredAt });

        builder.HasOne<LotBatch>()
            .WithMany()
            .HasForeignKey(x => x.LotBatchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
