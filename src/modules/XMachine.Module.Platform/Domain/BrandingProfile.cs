using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Platform.Domain;

public sealed class BrandingProfile : TenantAuditableEntity
{
    public string DisplayName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

