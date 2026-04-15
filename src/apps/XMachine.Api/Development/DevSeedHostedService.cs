using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Commercial.Domain;
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

        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Dev seed completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

