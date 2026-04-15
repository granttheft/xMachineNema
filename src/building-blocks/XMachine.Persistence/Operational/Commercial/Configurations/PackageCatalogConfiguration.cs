using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Commercial.Domain;

namespace XMachine.Persistence.Operational.Commercial.Configurations;

internal sealed class PackageCatalogConfiguration : IEntityTypeConfiguration<PackageCatalog>
{
    public void Configure(EntityTypeBuilder<PackageCatalog> builder)
    {
        builder.ToTable("package_catalogs", "commercial");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

