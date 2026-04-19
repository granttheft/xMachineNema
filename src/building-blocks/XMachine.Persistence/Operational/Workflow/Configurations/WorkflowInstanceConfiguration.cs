using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Workflow.Domain;

namespace XMachine.Persistence.Operational.Workflow.Configurations;

internal sealed class WorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("workflow_instances", "workflow");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.WorkflowDefinitionId).HasColumnName("workflow_definition_id").IsRequired();
        builder.Property(x => x.ReferenceType).HasColumnName("reference_type").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReferenceId).HasColumnName("reference_id").IsRequired();
        builder.Property(x => x.WorkflowState).HasColumnName("workflow_state").IsRequired();
        builder.Property(x => x.StartedAt).HasColumnName("started_at").IsRequired();
        builder.Property(x => x.EndedAt).HasColumnName("ended_at");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.ReferenceType, x.ReferenceId });

        builder.HasOne<WorkflowDefinition>()
            .WithMany()
            .HasForeignKey(x => x.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
