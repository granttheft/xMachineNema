using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMachine.Module.Auth.Domain;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational.Mes.Configurations;

internal sealed class EmployeeAssignmentConfiguration : IEntityTypeConfiguration<EmployeeAssignment>
{
    public void Configure(EntityTypeBuilder<EmployeeAssignment> builder)
    {
        builder.ToTable("employee_assignments", "mes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.UserAccountId).HasColumnName("user_account_id").IsRequired();
        builder.Property(x => x.ShiftId).HasColumnName("shift_id");
        builder.Property(x => x.LineId).HasColumnName("line_id");
        builder.Property(x => x.MachineId).HasColumnName("machine_id");

        builder.Property(x => x.AssignedFrom).HasColumnName("assigned_from");
        builder.Property(x => x.AssignedTo).HasColumnName("assigned_to");
        builder.Property(x => x.AssignmentRole).HasColumnName("assignment_role").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(x => new { x.TenantId, x.UserAccountId, x.ShiftId });

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Shift>()
            .WithMany()
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Line>()
            .WithMany()
            .HasForeignKey(x => x.LineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
