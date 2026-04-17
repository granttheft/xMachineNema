using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class ProductionOrderConfiguration : IEntityTypeConfiguration<ProductionOrder>
{
    public void Configure(EntityTypeBuilder<ProductionOrder> builder)
    {
        builder.ToTable("production_orders", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.SiteId).HasColumnName("site_id");
        builder.Property(x => x.LineId).HasColumnName("line_id");

        builder.Property(x => x.OrderNo).HasColumnName("order_no").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ProductCode).HasColumnName("product_code").HasMaxLength(128).IsRequired();
        builder.Property(x => x.QuantityPlanned).HasColumnName("quantity_planned").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.QuantityCompleted).HasColumnName("quantity_completed").HasPrecision(18, 4).IsRequired();

        builder.Property(x => x.OrderStatus).HasColumnName("order_status").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.PlannedStartAt).HasColumnName("planned_start_at");
        builder.Property(x => x.PlannedEndAt).HasColumnName("planned_end_at");
        builder.Property(x => x.ActualStartAt).HasColumnName("actual_start_at");
        builder.Property(x => x.ActualEndAt).HasColumnName("actual_end_at");

        builder.Property(x => x.SourceSystem).HasColumnName("source_system").HasMaxLength(64);
        builder.Property(x => x.SourceReference).HasColumnName("source_reference").HasMaxLength(256);

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.OrderNo }).IsUnique();

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => x.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Line>()
            .WithMany()
            .HasForeignKey(x => x.LineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
