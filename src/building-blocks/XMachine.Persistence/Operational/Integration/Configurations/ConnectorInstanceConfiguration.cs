using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Integration.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Integration.Configurations;

internal sealed class ConnectorInstanceConfiguration : IEntityTypeConfiguration<ConnectorInstance>
{
    public void Configure(EntityTypeBuilder<ConnectorInstance> builder)
    {
        builder.ToTable("connector_instances", "integration");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ConnectorDefinitionId).HasColumnName("connector_definition_id").IsRequired();
        builder.Property(x => x.SiteId).HasColumnName("site_id");
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.ConfigurationJson).HasColumnName("configuration_json");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasIndex(x => x.ConnectorDefinitionId);

        builder.HasOne<ConnectorDefinition>()
            .WithMany()
            .HasForeignKey(x => x.ConnectorDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => x.SiteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
