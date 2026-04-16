using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Commercial.Domain;
using XMachine.Module.Integration.Domain;
using XMachine.Module.Platform.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Api.Development;

internal sealed class DevSeedHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DevSeedHostedService> _logger;

    public DevSeedHostedService(
        IServiceProvider services,
        IWebHostEnvironment env,
        IConfiguration configuration,
        ILogger<DevSeedHostedService> logger)
    {
        _services = services;
        _env = env;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
        {
            return;
        }

        var enabled = _configuration.GetValue("XMachine:Seed:Enabled", false);
        if (!enabled)
        {
            _logger.LogInformation("Dev seed disabled (XMachine:Seed:Enabled=false).");
            return;
        }

        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XMachineDbContext>();

        await db.Database.MigrateAsync(cancellationToken);

        if (await db.Tenants.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Dev seed skipped (tenant already exists).");
            return;
        }

        var tenant = new Tenant
        {
            Code = "demo",
            Name = "Demo Tenant",
            Status = EntityStatus.Active,
        };

        var enterprise = new Enterprise
        {
            TenantId = tenant.Id,
            Code = "ENT-1",
            Name = "Demo Enterprise",
            Status = EntityStatus.Active,
        };

        var site = new Site
        {
            TenantId = tenant.Id,
            EnterpriseId = enterprise.Id,
            Code = "SITE-1",
            Name = "Demo Site",
            Status = EntityStatus.Active,
        };

        var line1 = new Line
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            Code = "LINE-1",
            Name = "Line 1",
            Status = EntityStatus.Active,
        };

        var line2 = new Line
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            Code = "LINE-2",
            Name = "Line 2",
            Status = EntityStatus.Active,
        };

        var machines = new[]
        {
            new Machine { TenantId = tenant.Id, LineId = line1.Id, Code = "M-101", Name = "Machine 101", Status = EntityStatus.Active },
            new Machine { TenantId = tenant.Id, LineId = line1.Id, Code = "M-102", Name = "Machine 102", Status = EntityStatus.Active },
            new Machine { TenantId = tenant.Id, LineId = line2.Id, Code = "M-201", Name = "Machine 201", Status = EntityStatus.Active },
            new Machine { TenantId = tenant.Id, LineId = line2.Id, Code = "M-202", Name = "Machine 202", Status = EntityStatus.Active },
        };

        var roleAdmin = new Role
        {
            TenantId = tenant.Id,
            Code = "admin",
            Name = "Administrator",
            Status = EntityStatus.Active,
        };

        var roleViewer = new Role
        {
            TenantId = tenant.Id,
            Code = "viewer",
            Name = "Viewer",
            Status = EntityStatus.Active,
        };

        var modulePlatform = new CommercialModule { Code = "platform", Name = "Platform", Status = EntityStatus.Active };
        var moduleCoreMes = new CommercialModule { Code = "core-mes", Name = "Core MES", Status = EntityStatus.Active };
        var moduleIntegration = new CommercialModule { Code = "integration", Name = "Integration", Status = EntityStatus.Active };

        var license = new License
        {
            TenantId = tenant.Id,
            LicenseType = LicenseType.Trial,
            ValidFrom = DateTimeOffset.UtcNow,
            ValidTo = DateTimeOffset.UtcNow.AddDays(14),
            Status = EntityStatus.Active,
        };

        var activations = new[]
        {
            new TenantModuleActivation { TenantId = tenant.Id, ModuleId = modulePlatform.Id, Status = EntityStatus.Active },
            new TenantModuleActivation { TenantId = tenant.Id, ModuleId = moduleCoreMes.Id, Status = EntityStatus.Active },
            new TenantModuleActivation { TenantId = tenant.Id, ModuleId = moduleIntegration.Id, Status = EntityStatus.Active },
        };

        var licensedLines = new[]
        {
            new LicensedLine { TenantId = tenant.Id, LineId = line1.Id, Status = EntityStatus.Active },
            new LicensedLine { TenantId = tenant.Id, LineId = line2.Id, Status = EntityStatus.Active },
        };

        var defOpcua = new ConnectorDefinition
        {
            TenantId = tenant.Id,
            Code = "opcua",
            Name = "OPC UA",
            Category = "plc",
            Direction = ConnectorDirection.Inbound,
            SupportsRead = true,
            SupportsWrite = false,
            Status = EntityStatus.Active,
        };

        var defS7 = new ConnectorDefinition
        {
            TenantId = tenant.Id,
            Code = "s7",
            Name = "Siemens S7",
            Category = "plc",
            Direction = ConnectorDirection.Inbound,
            SupportsRead = true,
            SupportsWrite = false,
            Status = EntityStatus.Active,
        };

        var defModbusTcp = new ConnectorDefinition
        {
            TenantId = tenant.Id,
            Code = "modbus_tcp",
            Name = "Modbus TCP",
            Category = "plc",
            Direction = ConnectorDirection.Inbound,
            SupportsRead = true,
            SupportsWrite = true,
            Status = EntityStatus.Active,
        };

        var defSap = new ConnectorDefinition
        {
            TenantId = tenant.Id,
            Code = "sap",
            Name = "SAP (placeholder)",
            Category = "erp",
            Direction = ConnectorDirection.Bidirectional,
            SupportsRead = true,
            SupportsWrite = true,
            Status = EntityStatus.Active,
        };

        var defRest = new ConnectorDefinition
        {
            TenantId = tenant.Id,
            Code = "rest",
            Name = "REST / HTTP",
            Category = "http",
            Direction = ConnectorDirection.Bidirectional,
            SupportsRead = true,
            SupportsWrite = true,
            Status = EntityStatus.Active,
        };

        var instOpcLine = new ConnectorInstance
        {
            TenantId = tenant.Id,
            ConnectorDefinitionId = defOpcua.Id,
            SiteId = site.Id,
            Code = "demo_line_opcua",
            Name = "Demo OPC UA instance",
            ConfigurationJson = """{"endpointUrl":"opc.tcp://localhost:4840/xmachine-dev"}""",
            Status = EntityStatus.Active,
        };

        var instSapSite = new ConnectorInstance
        {
            TenantId = tenant.Id,
            ConnectorDefinitionId = defSap.Id,
            SiteId = site.Id,
            Code = "demo_site_sap",
            Name = "Demo SAP adapter",
            ConfigurationJson = "{}",
            Status = EntityStatus.Active,
        };

        var instRestSite = new ConnectorInstance
        {
            TenantId = tenant.Id,
            ConnectorDefinitionId = defRest.Id,
            SiteId = site.Id,
            Code = "demo_site_rest",
            Name = "Demo REST poller",
            ConfigurationJson = """{"baseUrl":"https://example.invalid"}""",
            Status = EntityStatus.Active,
        };

        var mapProfileOpc = new MappingProfile
        {
            TenantId = tenant.Id,
            ConnectorInstanceId = instOpcLine.Id,
            Name = "default",
            Version = 1,
            Status = EntityStatus.Active,
        };

        var mapProfileSap = new MappingProfile
        {
            TenantId = tenant.Id,
            ConnectorInstanceId = instSapSite.Id,
            Name = "default",
            Version = 1,
            Status = EntityStatus.Active,
        };

        var mapRules = new[]
        {
            new MappingRule
            {
                MappingProfileId = mapProfileOpc.Id,
                SourceField = "ns=2;s=Demo/SpindleSpeed",
                LogicalField = "spindle_rpm_raw",
                CanonicalField = "machine.spindle_speed_rpm",
                SortOrder = 10,
                Status = EntityStatus.Active,
            },
            new MappingRule
            {
                MappingProfileId = mapProfileOpc.Id,
                SourceField = "ns=2;s=Demo/PartCount",
                LogicalField = "parts_produced_raw",
                CanonicalField = "production.part_count",
                SortOrder = 20,
                Status = EntityStatus.Active,
            },
            new MappingRule
            {
                MappingProfileId = mapProfileSap.Id,
                SourceField = "AUFNR",
                LogicalField = "sap_order_number",
                CanonicalField = "order.number",
                TransformKind = "trim",
                SortOrder = 10,
                Status = EntityStatus.Active,
            },
        };

        var assetTag = new AssetTagMap
        {
            TenantId = tenant.Id,
            ConnectorInstanceId = instOpcLine.Id,
            MachineId = machines[0].Id,
            SourceAddress = "ns=2;s=Demo/ActiveProgram",
            CanonicalField = "machine.active_program_name",
            Status = EntityStatus.Active,
        };

        var syncJob = new SyncJob
        {
            TenantId = tenant.Id,
            ConnectorInstanceId = instOpcLine.Id,
            JobType = "poll",
            JobStatus = SyncJobStatus.Pending,
            PayloadJson = """{"intervalSeconds":30}""",
        };

        db.Add(tenant);
        db.Add(enterprise);
        db.Add(site);
        db.AddRange(line1, line2);
        db.AddRange(machines);
        db.AddRange(roleAdmin, roleViewer);
        db.AddRange(modulePlatform, moduleCoreMes, moduleIntegration);
        db.Add(license);
        db.AddRange(activations);
        db.AddRange(licensedLines);
        db.AddRange(defOpcua, defS7, defModbusTcp, defSap, defRest);
        db.AddRange(instOpcLine, instSapSite, instRestSite);
        db.AddRange(mapProfileOpc, mapProfileSap);
        db.AddRange(mapRules);
        db.Add(assetTag);
        db.Add(syncJob);

        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Dev seed completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

