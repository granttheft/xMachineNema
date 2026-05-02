using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Planning.Domain;

namespace XMachine.Persistence.Operational.Planning.Configurations;

internal sealed class ProductionPlanConfiguration : IEntityTypeConfiguration<ProductionPlan>
{
    public void Configure(EntityTypeBuilder<ProductionPlan> builder)
    {
        builder.ToTable("production_plans", "planning");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();

        builder.Property(x => x.PlanNo).HasColumnName("plan_no").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(4000);

        builder.Property(x => x.PlanStatus).HasColumnName("plan_status").IsRequired();
        builder.Property(x => x.Priority).HasColumnName("priority").IsRequired();

        builder.Property(x => x.PlannedStartAt).HasColumnName("planned_start_at").IsRequired();
        builder.Property(x => x.PlannedEndAt).HasColumnName("planned_end_at").IsRequired();

        builder.Property(x => x.SiteId).HasColumnName("site_id");
        builder.Property(x => x.LineId).HasColumnName("line_id");

        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.ApprovedByUserId).HasColumnName("approved_by_user_id");
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at");
        builder.Property(x => x.ApprovalNotes).HasColumnName("approval_notes").HasMaxLength(2000);
        builder.Property(x => x.CancellationReason).HasColumnName("cancellation_reason").HasMaxLength(2000);

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.PlanStatus });
        builder.HasIndex(x => new { x.TenantId, x.PlannedStartAt });
        builder.HasIndex(x => new { x.TenantId, x.PlanNo }).IsUnique();
    }
}
