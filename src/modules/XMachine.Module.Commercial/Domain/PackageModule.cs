using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Commercial.Domain;

public sealed class PackageModule : AuditableEntity
{
    public Guid PackageCatalogId { get; set; }
    public Guid ModuleId { get; set; }
}

