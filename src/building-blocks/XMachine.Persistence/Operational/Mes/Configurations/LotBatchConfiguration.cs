using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class LotBatchConfiguration : IEntityTypeConfiguration<LotBatch>
{
    public void Configure(EntityTypeBuilder<LotBatch> builder)
    {
        builder.ToTable("lot_batches", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(x => x.LineId).HasColumnName("line_id");
        builder.Property(x => x.MachineId).HasColumnName("machine_id");

        builder.Property(x => x.LotNo).HasColumnName("lot_no").HasMaxLength(64).IsRequired();
        builder.Property(x => x.LotStatus).HasColumnName("lot_status").IsRequired();
        builder.Property(x => x.TargetQuantity).HasColumnName("target_quantity").HasPrecision(18, 4);
        builder.Property(x => x.QuantityGood).HasColumnName("quantity_good").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.StartedAt).HasColumnName("started_at");
        builder.Property(x => x.ClosedAt).HasColumnName("closed_at");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.LotNo }).IsUnique();

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
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
