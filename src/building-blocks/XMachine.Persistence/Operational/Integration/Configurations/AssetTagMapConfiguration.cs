using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Integration.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Integration.Configurations;

internal sealed class AssetTagMapConfiguration : IEntityTypeConfiguration<AssetTagMap>
{
    public void Configure(EntityTypeBuilder<AssetTagMap> builder)
    {
        builder.ToTable("asset_tag_maps", "integration");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ConnectorInstanceId).HasColumnName("connector_instance_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id");
        builder.Property(x => x.SourceAddress).HasColumnName("source_address").HasMaxLength(512).IsRequired();
        builder.Property(x => x.CanonicalField).HasColumnName("canonical_field").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.ConnectorInstanceId, x.SourceAddress });

        builder.HasOne<ConnectorInstance>()
            .WithMany()
            .HasForeignKey(x => x.ConnectorInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
