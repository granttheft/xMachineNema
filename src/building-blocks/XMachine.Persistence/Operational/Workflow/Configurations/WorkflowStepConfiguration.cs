using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Workflow.Domain;

namespace XMachine.Persistence.Operational.Workflow.Configurations;

internal sealed class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
{
    public void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.ToTable("workflow_steps", "workflow");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.WorkflowDefinitionId).HasColumnName("workflow_definition_id").IsRequired();
        builder.Property(x => x.SequenceNo).HasColumnName("sequence_no").IsRequired();
        builder.Property(x => x.RoleCode).HasColumnName("role_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ApprovalMode).HasColumnName("approval_mode").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.WorkflowDefinitionId, x.SequenceNo }).IsUnique();

        builder.HasOne<WorkflowDefinition>()
            .WithMany()
            .HasForeignKey(x => x.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
