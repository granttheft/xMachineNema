using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class EmployeeAssignment : TenantAuditableEntity
{
    public Guid UserAccountId { get; set; }
    public Guid? ShiftId { get; set; }

    public Guid? LineId { get; set; }
    public Guid? MachineId { get; set; }

    public DateTimeOffset? AssignedFrom { get; set; }
    public DateTimeOffset? AssignedTo { get; set; }

    public string AssignmentRole { get; set; } = "operator";

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
