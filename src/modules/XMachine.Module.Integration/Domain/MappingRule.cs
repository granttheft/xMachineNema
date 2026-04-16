using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Integration.Domain;

public sealed class MappingRule : AuditableEntity
{
    public Guid MappingProfileId { get; set; }
    public string SourceField { get; set; } = string.Empty;
    public string? LogicalField { get; set; }
    public string CanonicalField { get; set; } = string.Empty;
    public string? TransformKind { get; set; }
    public int SortOrder { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
