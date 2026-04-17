using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class ProductionOperationConfiguration : IEntityTypeConfiguration<ProductionOperation>
{
    public void Configure(EntityTypeBuilder<ProductionOperation> builder)
    {
        builder.ToTable("production_operations", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id").IsRequired();

        builder.Property(x => x.SequenceNo).HasColumnName("sequence_no").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();

        builder.Property(x => x.LineId).HasColumnName("line_id");
        builder.Property(x => x.MachineId).HasColumnName("machine_id");

        builder.Property(x => x.OperationStatus).HasColumnName("operation_status").IsRequired();
        builder.Property(x => x.QuantityPlanned).HasColumnName("quantity_planned").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.QuantityCompleted).HasColumnName("quantity_completed").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.ProductionOrderId, x.SequenceNo }).IsUnique();

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

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
