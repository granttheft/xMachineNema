using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Production.Domain;

public sealed class OperatorDeclaration : TenantAuditableEntity
{
    public Guid JobExecutionId { get; set; }
    public Guid MachineId { get; set; }
    public Guid OperatorId { get; set; }

    public int DeclaredQty { get; set; }
    public int ScrapQty { get; set; }
    public int DefectQty { get; set; }

    public string? Notes { get; set; }
    public DateTimeOffset DeclaredAt { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
