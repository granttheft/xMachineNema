using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Commercial.Domain;

namespace XMachine.Persistence.Operational.Commercial.Configurations;

internal sealed class LicensedLineConfiguration : IEntityTypeConfiguration<LicensedLine>
{
    public void Configure(EntityTypeBuilder<LicensedLine> builder)
    {
        builder.ToTable("licensed_lines", "commercial");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.LineId).HasColumnName("line_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.LineId }).IsUnique();
    }
}

