using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.MES.Domain;

public sealed class RecipeParameter : AuditableEntity
{
    public Guid RecipeId { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public string? Unit { get; set; }
    public string? DefaultValue { get; set; }
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }
    public int SortOrder { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
