using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Eventing.Domain;

namespace XMachine.Persistence.Operational.Eventing.Configurations;

internal sealed class KpiResultConfiguration : IEntityTypeConfiguration<KpiResult>
{
    public void Configure(EntityTypeBuilder<KpiResult> builder)
    {
        builder.ToTable("kpi_results", "eventing");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.KpiDefinitionId).HasColumnName("kpi_definition_id").IsRequired();

        builder.Property(x => x.ScopeType).HasColumnName("scope_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ScopeId).HasColumnName("scope_id").IsRequired();

        builder.Property(x => x.PeriodStart).HasColumnName("period_start").IsRequired();
        builder.Property(x => x.PeriodEnd).HasColumnName("period_end").IsRequired();
        builder.Property(x => x.Value).HasColumnName("value").HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.KpiDefinitionId, x.ScopeType, x.ScopeId, x.PeriodStart });

        builder.HasOne<KpiDefinition>()
            .WithMany()
            .HasForeignKey(x => x.KpiDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
