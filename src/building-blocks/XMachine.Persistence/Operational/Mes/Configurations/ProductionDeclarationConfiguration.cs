using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class ProductionDeclarationConfiguration : IEntityTypeConfiguration<ProductionDeclaration>
{
    public void Configure(EntityTypeBuilder<ProductionDeclaration> builder)
    {
        builder.ToTable("production_declarations", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.LotBatchId).HasColumnName("lot_batch_id").IsRequired();
        builder.Property(x => x.GoodQuantity).HasColumnName("good_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.DeclaredAt).HasColumnName("declared_at").IsRequired();
        builder.Property(x => x.LineId).HasColumnName("line_id");
        builder.Property(x => x.MachineId).HasColumnName("machine_id");
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2048);
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
