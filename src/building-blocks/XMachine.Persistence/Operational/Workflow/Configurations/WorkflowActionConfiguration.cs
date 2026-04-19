using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Workflow.Domain;

namespace XMachine.Persistence.Operational.Workflow.Configurations;

internal sealed class WorkflowActionConfiguration : IEntityTypeConfiguration<WorkflowAction>
{
    public void Configure(EntityTypeBuilder<WorkflowAction> builder)
    {
        builder.ToTable("workflow_actions", "workflow");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").IsRequired();
        builder.Property(x => x.WorkflowStepId).HasColumnName("workflow_step_id").IsRequired();
        builder.Property(x => x.ActionType).HasColumnName("action_type").IsRequired();
        builder.Property(x => x.ActionBy).HasColumnName("action_by");
        builder.Property(x => x.ActionTime).HasColumnName("action_time").IsRequired();
        builder.Property(x => x.Comment).HasColumnName("comment").HasMaxLength(2000);
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.WorkflowInstanceId, x.ActionTime });

        builder.HasOne<WorkflowInstance>()
            .WithMany()
            .HasForeignKey(x => x.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<WorkflowStep>()
            .WithMany()
            .HasForeignKey(x => x.WorkflowStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ActionBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
