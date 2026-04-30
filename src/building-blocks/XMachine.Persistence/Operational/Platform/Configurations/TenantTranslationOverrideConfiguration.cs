using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Platform.Configurations;

internal sealed class TenantTranslationOverrideConfiguration : IEntityTypeConfiguration<TenantTranslationOverride>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TenantTranslationOverride> builder)
    {
        builder.ToTable("tenant_translation_overrides", "platform");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.LanguageCode).HasColumnName("language_code").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Key).HasColumnName("key").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Value).HasColumnName("value").HasMaxLength(4096).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.LanguageCode });
        builder.HasIndex(x => new { x.TenantId, x.LanguageCode, x.Key }).IsUnique();
    }
}
