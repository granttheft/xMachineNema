using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Integration.Domain;

namespace XMachine.Persistence.Operational.Integration.Configurations;

internal sealed class ConnectorDefinitionConfiguration : IEntityTypeConfiguration<ConnectorDefinition>
{
    public void Configure(EntityTypeBuilder<ConnectorDefinition> builder)
    {
        builder.ToTable("connector_definitions", "integration");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Direction).HasColumnName("direction").IsRequired();
        builder.Property(x => x.SupportsRead).HasColumnName("supports_read").IsRequired();
        builder.Property(x => x.SupportsWrite).HasColumnName("supports_write").IsRequired();
        builder.Property(x => x.CapabilitiesJson).HasColumnName("capabilities_json");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
