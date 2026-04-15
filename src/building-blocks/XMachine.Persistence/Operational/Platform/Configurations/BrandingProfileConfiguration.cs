using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Platform.Configurations;

internal sealed class BrandingProfileConfiguration : IEntityTypeConfiguration<BrandingProfile>
{
    public void Configure(EntityTypeBuilder<BrandingProfile> builder)
    {
        builder.ToTable("branding_profiles", "platform");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.LogoUrl).HasColumnName("logo_url").HasMaxLength(2048);
        builder.Property(x => x.PrimaryColor).HasColumnName("primary_color").HasMaxLength(32);
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => x.TenantId);
    }
}

