using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Commercial.Domain;

namespace XMachine.Persistence.Operational.Commercial.Configurations;

internal sealed class PackageModuleConfiguration : IEntityTypeConfiguration<PackageModule>
{
    public void Configure(EntityTypeBuilder<PackageModule> builder)
    {
        builder.ToTable("package_modules", "commercial");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.PackageCatalogId).HasColumnName("package_catalog_id").IsRequired();
        builder.Property(x => x.ModuleId).HasColumnName("module_id").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.PackageCatalogId, x.ModuleId }).IsUnique();

        builder.HasOne<PackageCatalog>()
            .WithMany()
            .HasForeignKey(x => x.PackageCatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<CommercialModule>()
            .WithMany()
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

