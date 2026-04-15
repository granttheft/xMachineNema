using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Commercial.Domain;

namespace XMachine.Persistence.Operational.Commercial.Configurations;

internal sealed class TenantModuleActivationConfiguration : IEntityTypeConfiguration<TenantModuleActivation>
{
    public void Configure(EntityTypeBuilder<TenantModuleActivation> builder)
    {
        builder.ToTable("tenant_module_activations", "commercial");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(x => x.ActivatedAt).HasColumnName("activated_at").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.ModuleId }).IsUnique();

        builder.HasOne<CommercialModule>()
            .WithMany()
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

