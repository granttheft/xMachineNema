using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Integration.Domain;

namespace XMachine.Persistence.Operational.Integration.Configurations;

internal sealed class MappingProfileConfiguration : IEntityTypeConfiguration<MappingProfile>
{
    public void Configure(EntityTypeBuilder<MappingProfile> builder)
    {
        builder.ToTable("mapping_profiles", "integration");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ConnectorInstanceId).HasColumnName("connector_instance_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.ConnectorInstanceId, x.Name, x.Version }).IsUnique();

        builder.HasOne<ConnectorInstance>()
            .WithMany()
            .HasForeignKey(x => x.ConnectorInstanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
