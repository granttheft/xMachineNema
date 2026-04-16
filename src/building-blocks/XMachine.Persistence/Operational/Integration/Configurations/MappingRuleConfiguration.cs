using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Integration.Domain;

namespace XMachine.Persistence.Operational.Integration.Configurations;

internal sealed class MappingRuleConfiguration : IEntityTypeConfiguration<MappingRule>
{
    public void Configure(EntityTypeBuilder<MappingRule> builder)
    {
        builder.ToTable("mapping_rules", "integration");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.MappingProfileId).HasColumnName("mapping_profile_id").IsRequired();
        builder.Property(x => x.SourceField).HasColumnName("source_field").HasMaxLength(512).IsRequired();
        builder.Property(x => x.LogicalField).HasColumnName("logical_field").HasMaxLength(256);
        builder.Property(x => x.CanonicalField).HasColumnName("canonical_field").HasMaxLength(256).IsRequired();
        builder.Property(x => x.TransformKind).HasColumnName("transform_kind").HasMaxLength(64);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.MappingProfileId, x.SortOrder });

        builder.HasOne<MappingProfile>()
            .WithMany()
            .HasForeignKey(x => x.MappingProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
