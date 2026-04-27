using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Production.Domain;

namespace XMachine.Persistence.Operational.Production.Configurations;

internal sealed class OperatorDeclarationConfiguration : IEntityTypeConfiguration<OperatorDeclaration>
{
    public void Configure(EntityTypeBuilder<OperatorDeclaration> builder)
    {
        builder.ToTable("operator_declarations", "production");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.JobExecutionId).HasColumnName("job_execution_id").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id").IsRequired();
        builder.Property(x => x.OperatorId).HasColumnName("operator_id").IsRequired();

        builder.Property(x => x.DeclaredQty).HasColumnName("declared_qty").IsRequired();
        builder.Property(x => x.ScrapQty).HasColumnName("scrap_qty").IsRequired();
        builder.Property(x => x.DefectQty).HasColumnName("defect_qty").IsRequired();

        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2048);
        builder.Property(x => x.DeclaredAt).HasColumnName("declared_at").IsRequired();

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.JobExecutionId });
        builder.HasIndex(x => new { x.TenantId, x.OperatorId });
        builder.HasIndex(x => new { x.TenantId, x.DeclaredAt });

        builder.HasOne<JobExecution>()
            .WithMany()
            .HasForeignKey(x => x.JobExecutionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
