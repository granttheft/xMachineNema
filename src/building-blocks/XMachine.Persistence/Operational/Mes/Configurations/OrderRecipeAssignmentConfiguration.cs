using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.MES.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class OrderRecipeAssignmentConfiguration : IEntityTypeConfiguration<OrderRecipeAssignment>
{
    public void Configure(EntityTypeBuilder<OrderRecipeAssignment> builder)
    {
        builder.ToTable("order_recipe_assignments", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(x => x.RecipeId).HasColumnName("recipe_id").IsRequired();
        builder.Property(x => x.RecipeVersionAssigned).HasColumnName("recipe_version_assigned").IsRequired();
        builder.Property(x => x.AssignedAt).HasColumnName("assigned_at").IsRequired();
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.ProductionOrderId, x.RecipeId });

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
