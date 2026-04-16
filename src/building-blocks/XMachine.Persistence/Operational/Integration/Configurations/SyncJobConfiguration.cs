using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Integration.Domain;

namespace XMachine.Persistence.Operational.Integration.Configurations;

internal sealed class SyncJobConfiguration : IEntityTypeConfiguration<SyncJob>
{
    public void Configure(EntityTypeBuilder<SyncJob> builder)
    {
        builder.ToTable("sync_jobs", "integration");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ConnectorInstanceId).HasColumnName("connector_instance_id").IsRequired();
        builder.Property(x => x.JobType).HasColumnName("job_type").HasMaxLength(64).IsRequired();
        builder.Property(x => x.JobStatus).HasColumnName("job_status").IsRequired();
        builder.Property(x => x.PayloadJson).HasColumnName("payload_json");
        builder.Property(x => x.StartedAt).HasColumnName("started_at");
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at");
        builder.Property(x => x.ErrorMessage).HasColumnName("error_message").HasMaxLength(2048);

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.ConnectorInstanceId, x.JobStatus });

        builder.HasOne<ConnectorInstance>()
            .WithMany()
            .HasForeignKey(x => x.ConnectorInstanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
