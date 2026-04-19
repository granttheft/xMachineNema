using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Commercial.Domain;
using XMachine.Module.Eventing.Domain;
using XMachine.Module.Integration.Domain;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;
using XMachine.Module.Quality.Domain;
using XMachine.Module.Workflow.Domain;

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

    // integration (operational metadata)
    public DbSet<ConnectorDefinition> ConnectorDefinitions => Set<ConnectorDefinition>();
    public DbSet<ConnectorInstance> ConnectorInstances => Set<ConnectorInstance>();
    public DbSet<MappingProfile> MappingProfiles => Set<MappingProfile>();
    public DbSet<MappingRule> MappingRules => Set<MappingRule>();
    public DbSet<AssetTagMap> AssetTagMaps => Set<AssetTagMap>();
    public DbSet<SyncJob> SyncJobs => Set<SyncJob>();

    // mes (core manufacturing)
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<ProductionOperation> ProductionOperations => Set<ProductionOperation>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeParameter> RecipeParameters => Set<RecipeParameter>();
    public DbSet<OrderRecipeAssignment> OrderRecipeAssignments => Set<OrderRecipeAssignment>();
    public DbSet<LotBatch> LotBatches => Set<LotBatch>();
    public DbSet<MaterialConsumption> MaterialConsumptions => Set<MaterialConsumption>();
    public DbSet<ProductionDeclaration> ProductionDeclarations => Set<ProductionDeclaration>();
    public DbSet<ScrapDeclaration> ScrapDeclarations => Set<ScrapDeclaration>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<EmployeeAssignment> EmployeeAssignments => Set<EmployeeAssignment>();

    // quality
    public DbSet<QualityCheck> QualityChecks => Set<QualityCheck>();
    public DbSet<QualityMeasurement> QualityMeasurements => Set<QualityMeasurement>();
    public DbSet<Nonconformance> Nonconformances => Set<Nonconformance>();
    public DbSet<QualityDisposition> QualityDispositions => Set<QualityDisposition>();

    // eventing (alarms / downtime / OEE / KPI snapshots)
    public DbSet<AlarmEvent> AlarmEvents => Set<AlarmEvent>();
    public DbSet<DowntimeRecord> DowntimeRecords => Set<DowntimeRecord>();
    public DbSet<OeeSnapshot> OeeSnapshots => Set<OeeSnapshot>();
    public DbSet<KpiDefinition> KpiDefinitions => Set<KpiDefinition>();
    public DbSet<KpiResult> KpiResults => Set<KpiResult>();

    // workflow
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowAction> WorkflowActions => Set<WorkflowAction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(XMachineDbContext).Assembly);
    }
}

