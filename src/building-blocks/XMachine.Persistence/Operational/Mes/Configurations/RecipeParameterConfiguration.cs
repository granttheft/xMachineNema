using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class RecipeParameterConfiguration : IEntityTypeConfiguration<RecipeParameter>
{
    public void Configure(EntityTypeBuilder<RecipeParameter> builder)
    {
        builder.ToTable("recipe_parameters", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.RecipeId).HasColumnName("recipe_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.DataType).HasColumnName("data_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(32);
        builder.Property(x => x.DefaultValue).HasColumnName("default_value").HasMaxLength(512);
        builder.Property(x => x.MinValue).HasColumnName("min_value").HasMaxLength(128);
        builder.Property(x => x.MaxValue).HasColumnName("max_value").HasMaxLength(128);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.RecipeId, x.Code }).IsUnique();

        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
