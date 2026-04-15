using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Commercial.Domain;
using XMachine.Module.Platform.Domain;

namespace XMachine.Persistence.Operational;

public sealed class XMachineDbContext : DbContext
{
    public XMachineDbContext(DbContextOptions<XMachineDbContext> options) : base(options) { }

    // platform
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Enterprise> Enterprises => Set<Enterprise>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Line> Lines => Set<Line>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<BrandingProfile> BrandingProfiles => Set<BrandingProfile>();

    // auth
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();

    // commercial
    public DbSet<License> Licenses => Set<License>();
    public DbSet<CommercialModule> Modules => Set<CommercialModule>();
    public DbSet<TenantModuleActivation> TenantModuleActivations => Set<TenantModuleActivation>();
    public DbSet<LicensedLine> LicensedLines => Set<LicensedLine>();
    public DbSet<PackageCatalog> PackageCatalogs => Set<PackageCatalog>();
    public DbSet<PackageModule> PackageModules => Set<PackageModule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(XMachineDbContext).Assembly);
    }
}

