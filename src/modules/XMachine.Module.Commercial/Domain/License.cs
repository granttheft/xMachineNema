using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Commercial.Domain;

public sealed class License : TenantAuditableEntity
{
    public LicenseType LicenseType { get; set; } = LicenseType.Trial;
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }

    public string? LicenseKey { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

