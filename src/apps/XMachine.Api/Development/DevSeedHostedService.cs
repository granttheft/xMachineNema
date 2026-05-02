using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Auth.Security;
using XMachine.Module.Commercial.Domain;
using XMachine.Module.Engineering.Domain;
using XMachine.Module.Eventing.Domain;
using XMachine.Module.Integration.Domain;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;
using XMachine.Module.Planning.Domain;
using XMachine.Module.Production.Domain;
using XMachine.Module.Quality.Domain;
using XMachine.Module.Workflow.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Api.Development;

/// <summary>
/// Stable identifiers for demo production rows (M-102 running job, kiosk operator). Keep in sync with Web kiosk/dashboard constants.
/// </summary>
internal static class DevSeedProductionIds
{
    internal static readonly Guid DemoOperatorUser = Guid.Parse("c0000001-0000-4000-8000-000000000001");
    internal static readonly Guid MachineM102 = Guid.Parse("a0000001-0000-4000-8000-000000000102");
    internal static readonly Guid JobRunningOnM102 = Guid.Parse("b0000001-0000-4000-8000-000000000001");
}

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

        // Index map: 0 M-101, 1 M-102 (stable Id), 2 M-103, 3 M-104 (injection); 4 M-201, 5 M-202, 6 M-203, 7 M-204 (coating).
        var machines = new[]
        {
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line1.Id,
                Code = "M-101",
                Name = "Injection M-101",
                OperationalStatus = MachineOperationalStatus.Down,
                Status = EntityStatus.Active,
            },
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line1.Id,
                Code = "M-102",
                Name = "Injection M-102",
                OperationalStatus = MachineOperationalStatus.Running,
                Status = EntityStatus.Active,
            },
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line1.Id,
                Code = "M-103",
                Name = "Injection M-103",
                OperationalStatus = MachineOperationalStatus.Running,
                Status = EntityStatus.Active,
            },
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line1.Id,
                Code = "M-104",
                Name = "Injection M-104",
                OperationalStatus = MachineOperationalStatus.Idle,
                Status = EntityStatus.Active,
            },
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line2.Id,
                Code = "M-201",
                Name = "Coating M-201",
                OperationalStatus = MachineOperationalStatus.PmDue,
                Status = EntityStatus.Active,
            },
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line2.Id,
                Code = "M-202",
                Name = "Coating M-202",
                OperationalStatus = MachineOperationalStatus.Running,
                Status = EntityStatus.Active,
            },
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line2.Id,
                Code = "M-203",
                Name = "Coating M-203",
                OperationalStatus = MachineOperationalStatus.Idle,
                Status = EntityStatus.Active,
            },
            new Machine
            {
                TenantId = tenant.Id,
                LineId = line2.Id,
                Code = "M-204",
                Name = "Coating M-204",
                OperationalStatus = MachineOperationalStatus.Down,
                Status = EntityStatus.Active,
            },
        };

        machines[1].Id = DevSeedProductionIds.MachineM102;

        var roleSuperAdmin = MakeRole(tenant.Id, ApplicationRoles.SuperAdmin, "Super admin");
        var roleTenantAdmin = MakeRole(tenant.Id, ApplicationRoles.TenantAdmin, "Tenant admin");
        var roleSiteAdmin = MakeRole(tenant.Id, ApplicationRoles.SiteAdmin, "Site admin");
        var roleProductionManager = MakeRole(tenant.Id, ApplicationRoles.ProductionManager, "Production manager");
        var roleQuality = MakeRole(tenant.Id, ApplicationRoles.Quality, "Quality");
        var roleMaintenance = MakeRole(tenant.Id, ApplicationRoles.Maintenance, "Maintenance");
        var roleOperator = MakeRole(tenant.Id, ApplicationRoles.Operator, "Operator");

        var userOperator = new UserAccount
        {
            Id = DevSeedProductionIds.DemoOperatorUser,
            TenantId = tenant.Id,
            Username = "operator1",
            DisplayName = "Demo Operator",
            Email = "operator1@demo.local",
            Status = EntityStatus.Active,
        };

        var userOperator2 = new UserAccount
        {
            TenantId = tenant.Id,
            Username = "operator2",
            DisplayName = "Ali Kaya",
            Email = "operator2@demo.local",
            Status = EntityStatus.Active,
        };

        var userOperator3 = new UserAccount
        {
            TenantId = tenant.Id,
            Username = "operator3",
            DisplayName = "Fatma Demir",
            Email = "operator3@demo.local",
            Status = EntityStatus.Active,
        };

        var userEngineer1 = new UserAccount
        {
            TenantId = tenant.Id,
            Username = "engineer1",
            DisplayName = "Can Arslan",
            Email = "engineer1@demo.local",
            Status = EntityStatus.Active,
        };

        var userSupervisor = new UserAccount
        {
            TenantId = tenant.Id,
            Username = "supervisor1",
            DisplayName = "Demo Supervisor",
            Email = "supervisor1@demo.local",
            Status = EntityStatus.Active,
        };

        var userDevAdmin = new UserAccount
        {
            TenantId = tenant.Id,
            Username = "devadmin",
            DisplayName = "Development Super Admin",
            Email = "devadmin@demo.local",
            Status = EntityStatus.Active,
        };

        var roleAssignments = new[]
        {
            MakeRoleAssignment(tenant.Id, userDevAdmin.Id, roleSuperAdmin.Id, ScopeType.Tenant, tenant.Id),
            MakeRoleAssignment(tenant.Id, userSupervisor.Id, roleTenantAdmin.Id, ScopeType.Tenant, tenant.Id),
            MakeRoleAssignment(tenant.Id, userSupervisor.Id, roleProductionManager.Id, ScopeType.Tenant, tenant.Id),
            MakeRoleAssignment(tenant.Id, userOperator.Id, roleOperator.Id, ScopeType.Tenant, tenant.Id),
            MakeRoleAssignment(tenant.Id, userOperator2.Id, roleOperator.Id, ScopeType.Tenant, tenant.Id),
            MakeRoleAssignment(tenant.Id, userOperator3.Id, roleOperator.Id, ScopeType.Tenant, tenant.Id),
            MakeRoleAssignment(tenant.Id, userEngineer1.Id, roleMaintenance.Id, ScopeType.Tenant, tenant.Id),
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

        var defOpcua = MakeConnDef(tenant.Id, "opcua", "OPC UA", "plc", ConnectorDirection.Inbound, read: true, write: false);
        var defS7 = MakeConnDef(tenant.Id, "s7", "Siemens S7", "plc", ConnectorDirection.Inbound, read: true, write: false);
        var defModbusTcp = MakeConnDef(tenant.Id, "modbus_tcp", "Modbus TCP", "plc", ConnectorDirection.Inbound, read: true, write: true);
        var defSap = MakeConnDef(tenant.Id, "sap", "SAP (placeholder)", "erp", ConnectorDirection.Bidirectional, read: true, write: true);
        var defRest = MakeConnDef(tenant.Id, "rest", "REST / HTTP", "http", ConnectorDirection.Bidirectional, read: true, write: true);

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

        var recipeAsm = MakeRecipe(tenant.Id, "ASM-A", "Assembly recipe A", 1, "Demo assembly parameters");
        var recipeCoat = MakeRecipe(tenant.Id, "COAT-B", "Coating recipe B", 1, "Demo coating parameters");

        var recipeParams = new[]
        {
            MakeRecipeParam(recipeAsm.Id, "SPINDLE_RPM", "Target spindle rpm", "number", "rpm", "2400", "0", "8000", 10),
            MakeRecipeParam(recipeAsm.Id, "CYCLE_TIME_S", "Nominal cycle time", "number", "s", "45", "10", "300", 20),
            MakeRecipeParam(recipeAsm.Id, "TORQUE_NM", "Fastening torque", "number", "Nm", "35", "5", "120", 30),
            MakeRecipeParam(recipeCoat.Id, "OVEN_TEMP_C", "Oven temperature", "number", "C", "180", "120", "220", 10),
            MakeRecipeParam(recipeCoat.Id, "CURE_TIME_S", "Cure time", "number", "s", "900", "60", "7200", 20),
        };

        var order1 = MakeOrder(
            tenant.Id,
            site.Id,
            line1.Id,
            "WO-DEMO-1001",
            "SKU-BRACKET-01",
            500,
            120,
            ProductionOrderStatus.InProgress,
            DateTimeOffset.UtcNow.AddDays(-2),
            DateTimeOffset.UtcNow.AddDays(3),
            "ERP-WO-1001");

        var order2 = MakeOrder(
            tenant.Id,
            site.Id,
            line2.Id,
            "WO-DEMO-1002",
            "SKU-PANEL-02",
            200,
            25,
            ProductionOrderStatus.Released,
            DateTimeOffset.UtcNow.AddDays(-1),
            null,
            null);

        var order3 = MakeOrder(
            tenant.Id,
            site.Id,
            line1.Id,
            "WO-DEMO-1003",
            "SKU-COVER-03",
            600,
            0,
            ProductionOrderStatus.Released,
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(5),
            null);

        var order4 = MakeOrder(
            tenant.Id,
            site.Id,
            line2.Id,
            "WO-DEMO-1004",
            "SKU-SHAFT-04",
            400,
            0,
            ProductionOrderStatus.Draft,
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(10),
            null);

        var op1a = MakeOp(tenant.Id, order1.Id, 10, "ASM", "Assemble", line1.Id, machines[0].Id, ProductionOperationStatus.Running, 500, 120);
        var op1b = MakeOp(tenant.Id, order1.Id, 20, "PACK", "Pack", line1.Id, null, ProductionOperationStatus.Pending, 500, 0);
        var op2a = MakeOp(tenant.Id, order2.Id, 10, "COAT", "Coat", line2.Id, machines[5].Id, ProductionOperationStatus.Running, 200, 25);
        var op2b = MakeOp(tenant.Id, order2.Id, 20, "QC_VISUAL", "Visual check", line2.Id, null, ProductionOperationStatus.Pending, 200, 0);
        var op3a = MakeOp(tenant.Id, order3.Id, 10, "INJ", "Inject", line1.Id, machines[2].Id, ProductionOperationStatus.Running, 600, 178);
        var op4a = MakeOp(tenant.Id, order4.Id, 10, "COAT", "Coat prep", line2.Id, machines[6].Id, ProductionOperationStatus.Pending, 400, 0);

        var assign1 = MakeOrderRecipe(tenant.Id, order1.Id, recipeAsm.Id, recipeAsm.Version, DateTimeOffset.UtcNow.AddDays(-2));
        var assign2 = MakeOrderRecipe(tenant.Id, order2.Id, recipeCoat.Id, recipeCoat.Version, DateTimeOffset.UtcNow.AddDays(-1));
        var assign3 = MakeOrderRecipe(tenant.Id, order3.Id, recipeAsm.Id, recipeAsm.Version, DateTimeOffset.UtcNow.AddHours(-8));
        var assign4 = MakeOrderRecipe(tenant.Id, order4.Id, recipeCoat.Id, recipeCoat.Version, DateTimeOffset.UtcNow.AddHours(-2));

        var lot1 = MakeLot(tenant.Id, order1.Id, line1.Id, machines[0].Id, "LOT-DEMO-1001-A", 500, 120);
        var lot2 = MakeLot(tenant.Id, order2.Id, line2.Id, machines[5].Id, "LOT-DEMO-1002-A", 200, 25);
        var lot3 = MakeLot(tenant.Id, order3.Id, line1.Id, machines[2].Id, "LOT-DEMO-1003-A", 600, 178);

        var matCons = new[]
        {
            MakeMat(tenant.Id, lot1.Id, "RM-STEEL-01", "Steel blank", 130, "ea", DateTimeOffset.UtcNow.AddHours(-5)),
            MakeMat(tenant.Id, lot1.Id, "RM-SCREW-M4", "Screw M4", 520, "ea", DateTimeOffset.UtcNow.AddHours(-5)),
            MakeMat(tenant.Id, lot2.Id, "RM-PAINT-WHITE", "Paint white", 12, "kg", DateTimeOffset.UtcNow.AddHours(-2)),
            MakeMat(tenant.Id, lot3.Id, "RM-RESIN-03", "Cover resin", 190, "kg", DateTimeOffset.UtcNow.AddHours(-4)),
        };

        var prodDecl = new[]
        {
            MakeProdDecl(tenant.Id, lot1.Id, line1.Id, machines[0].Id, 40, DateTimeOffset.UtcNow.AddHours(-4), "Demo run 1"),
            MakeProdDecl(tenant.Id, lot1.Id, line1.Id, machines[0].Id, 50, DateTimeOffset.UtcNow.AddHours(-3), null),
            MakeProdDecl(tenant.Id, lot1.Id, line1.Id, machines[1].Id, 30, DateTimeOffset.UtcNow.AddHours(-2), null),
            MakeProdDecl(tenant.Id, lot2.Id, line2.Id, machines[5].Id, 25, DateTimeOffset.UtcNow.AddHours(-1), null),
            MakeProdDecl(tenant.Id, lot3.Id, line1.Id, machines[2].Id, 90, DateTimeOffset.UtcNow.AddHours(-3), "Shift A"),
            MakeProdDecl(tenant.Id, lot3.Id, line1.Id, machines[2].Id, 88, DateTimeOffset.UtcNow.AddHours(-1), "Shift B"),
        };

        var scrap1 = new ScrapDeclaration
        {
            TenantId = tenant.Id,
            LotBatchId = lot1.Id,
            ScrapQuantity = 3,
            ReasonCode = "DIMENSION",
            DeclaredAt = DateTimeOffset.UtcNow.AddHours(-2),
            Notes = "Demo scrap sample",
            Status = EntityStatus.Active,
        };

        var invMoves = new[]
        {
            new InventoryMovement
            {
                TenantId = tenant.Id,
                MovementType = InventoryMovementType.Issue,
                MaterialCode = "RM-STEEL-01",
                Quantity = 130,
                Unit = "ea",
                LotBatchId = lot1.Id,
                ProductionOrderId = order1.Id,
                ReferenceType = "lot_batch",
                ReferenceId = lot1.Id,
                OccurredAt = DateTimeOffset.UtcNow.AddHours(-5),
                Note = "Issue to lot",
                Status = EntityStatus.Active,
            },
            new InventoryMovement
            {
                TenantId = tenant.Id,
                MovementType = InventoryMovementType.Receipt,
                MaterialCode = "SF-BRACKET-01",
                Quantity = 50,
                Unit = "ea",
                ProductionOrderId = order1.Id,
                ReferenceType = "production_order",
                ReferenceId = order1.Id,
                OccurredAt = DateTimeOffset.UtcNow.AddHours(-1),
                Note = "Semi-finished receipt placeholder",
                Status = EntityStatus.Active,
            },
        };

        var shiftDay = DateOnly.FromDateTime(DateTime.UtcNow);
        var dayStart = DateTime.SpecifyKind(shiftDay.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var shiftMorning = MakeShift(tenant.Id, site.Id, line1.Id, "DAY-A1", "Line 1 morning", shiftDay, dayStart, 6, 14, WorkShiftLifecycle.Open, actualStartHourOffset: 6.083);
        var shiftAfternoon = MakeShift(tenant.Id, site.Id, line1.Id, "DAY-A2", "Line 1 afternoon", shiftDay, dayStart, 14, 22, WorkShiftLifecycle.Planned, null);
        var shiftMorning2 = MakeShift(tenant.Id, site.Id, line2.Id, "DAY-B1", "Line 2 morning", shiftDay, dayStart, 6, 14, WorkShiftLifecycle.Open, actualStartHourOffset: 6.05);
        var shiftAfternoon2 = MakeShift(tenant.Id, site.Id, line2.Id, "DAY-B2", "Line 2 afternoon", shiftDay, dayStart, 14, 22, WorkShiftLifecycle.Planned, null);

        var empAssign = new[]
        {
            MakeEmpAssign(tenant.Id, userOperator.Id, shiftMorning.Id, line1.Id, null, shiftMorning.PlannedStartAt, shiftMorning.PlannedEndAt, "operator"),
            MakeEmpAssign(tenant.Id, userSupervisor.Id, shiftMorning.Id, line1.Id, null, shiftMorning.PlannedStartAt, shiftMorning.PlannedEndAt, "supervisor"),
            MakeEmpAssign(tenant.Id, userOperator2.Id, shiftMorning2.Id, line2.Id, null, shiftMorning2.PlannedStartAt, shiftMorning2.PlannedEndAt, "operator"),
            MakeEmpAssign(tenant.Id, userOperator3.Id, shiftAfternoon2.Id, line2.Id, machines[5].Id, shiftAfternoon2.PlannedStartAt, shiftAfternoon2.PlannedEndAt, "operator"),
            MakeEmpAssign(tenant.Id, userEngineer1.Id, shiftAfternoon.Id, line1.Id, null, shiftAfternoon.PlannedStartAt, shiftAfternoon.PlannedEndAt, "maintenance"),
        };

        var qcLine = new QualityCheck
        {
            TenantId = tenant.Id,
            ProductionOrderId = order1.Id,
            LotBatchId = lot1.Id,
            MachineId = machines[0].Id,
            CheckType = "in_process",
            CheckTime = DateTimeOffset.UtcNow.AddHours(-2),
            CheckStatus = QualityCheckStatus.Passed,
            ApprovedBy = userSupervisor.Id,
            Status = EntityStatus.Active,
        };

        var qcFinal = new QualityCheck
        {
            TenantId = tenant.Id,
            ProductionOrderId = order2.Id,
            LotBatchId = lot2.Id,
            MachineId = machines[5].Id,
            CheckType = "final",
            CheckTime = DateTimeOffset.UtcNow.AddHours(-1),
            CheckStatus = QualityCheckStatus.Pending,
            Status = EntityStatus.Active,
        };

        var qualityMeasurements = new[]
        {
            MakeMeasurement(tenant.Id, qcLine.Id, "LENGTH_MM", "100.02", "100.00", "99.90", "100.10", QualityMeasurementResult.Pass),
            MakeMeasurement(tenant.Id, qcLine.Id, "WIDTH_MM", "50.08", "50.00", "49.85", "50.15", QualityMeasurementResult.Warning),
            MakeMeasurement(tenant.Id, qcLine.Id, "SURFACE", "OK", null, null, null, QualityMeasurementResult.Pass),
            MakeMeasurement(tenant.Id, qcFinal.Id, "GLOSS", "82", "80", "75", "90", QualityMeasurementResult.Pass),
            MakeMeasurement(tenant.Id, qcFinal.Id, "COLOR_DELTA", "0.6", "1.0", null, null, QualityMeasurementResult.NotEvaluated),
        };

        var ncDemo = new Nonconformance
        {
            TenantId = tenant.Id,
            QualityCheckId = qcLine.Id,
            NcCode = "NC-DEMO-001",
            Description = "Demo: measurement near upper tolerance band.",
            Severity = NonconformanceSeverity.Major,
            NcStatus = NonconformanceStatus.Closed,
            Status = EntityStatus.Active,
        };

        var dispRework = new QualityDisposition
        {
            TenantId = tenant.Id,
            NonconformanceId = ncDemo.Id,
            DispositionType = QualityDispositionType.Rework,
            DecisionTime = DateTimeOffset.UtcNow.AddMinutes(-45),
            DecidedBy = userSupervisor.Id,
            Status = EntityStatus.Active,
        };

        var alarmDrive = new AlarmEvent
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line1.Id,
            MachineId = machines[0].Id,
            AlarmCode = "DRV-OVERLOAD",
            AlarmText = "Demo: drive transient overload (placeholder).",
            Severity = AlarmSeverity.Warning,
            Category = "equipment",
            StartTime = DateTimeOffset.UtcNow.AddHours(-8),
            EndTime = DateTimeOffset.UtcNow.AddHours(-7).AddMinutes(12),
            DurationMs = 4_320_000,
            AckBy = userOperator.Id,
            AckTime = DateTimeOffset.UtcNow.AddHours(-7).AddMinutes(5),
            AlarmStatus = AlarmLifecycleStatus.Cleared,
            Status = EntityStatus.Active,
        };

        var alarmSite = new AlarmEvent
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            AlarmCode = "UTIL-AIR-LOW",
            AlarmText = "Demo: plant air pressure below nominal (placeholder).",
            Severity = AlarmSeverity.Error,
            Category = "utilities",
            StartTime = DateTimeOffset.UtcNow.AddHours(-3),
            EndTime = DateTimeOffset.UtcNow.AddHours(-2),
            DurationMs = 3_600_000,
            AckBy = userSupervisor.Id,
            AckTime = DateTimeOffset.UtcNow.AddHours(-2).AddMinutes(-20),
            AlarmStatus = AlarmLifecycleStatus.Cleared,
            Status = EntityStatus.Active,
        };

        var alarmLine2 = new AlarmEvent
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line2.Id,
            MachineId = machines[4].Id,
            AlarmCode = "PROC-TEMP-HIGH",
            AlarmText = "Demo: process temperature high (placeholder).",
            Severity = AlarmSeverity.Critical,
            Category = "process",
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-40),
            AlarmStatus = AlarmLifecycleStatus.Active,
            Status = EntityStatus.Active,
        };

        var alarmTemp102 = new AlarmEvent
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line1.Id,
            MachineId = machines[1].Id,
            AlarmCode = "TEMP-HIGH-102",
            AlarmText = "Injection barrel zone 2 above setpoint.",
            Severity = AlarmSeverity.Error,
            Category = "process",
            StartTime = DateTimeOffset.UtcNow.AddHours(-2),
            AlarmStatus = AlarmLifecycleStatus.Active,
            Status = EntityStatus.Active,
        };

        var alarmVib204 = new AlarmEvent
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line2.Id,
            MachineId = machines[7].Id,
            AlarmCode = "VIBRATION-M204",
            AlarmText = "Spindle vibration envelope exceeded nominal band.",
            Severity = AlarmSeverity.Warning,
            Category = "equipment",
            StartTime = DateTimeOffset.UtcNow.AddHours(-1),
            AlarmStatus = AlarmLifecycleStatus.Active,
            Status = EntityStatus.Active,
        };

        var alarmHyd101 = new AlarmEvent
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line1.Id,
            MachineId = machines[0].Id,
            AlarmCode = "HYD-PRESSURE",
            AlarmText = "Hydraulic pressure below minimum — production halted.",
            Severity = AlarmSeverity.Critical,
            Category = "equipment",
            StartTime = DateTimeOffset.UtcNow.AddHours(-3),
            AlarmStatus = AlarmLifecycleStatus.Active,
            Status = EntityStatus.Active,
        };

        var dtChangeover = new DowntimeRecord
        {
            TenantId = tenant.Id,
            MachineId = machines[0].Id,
            LineId = line1.Id,
            ProductionOrderId = order1.Id,
            DowntimeReasonCode = "CHG",
            DowntimeReasonText = "Demo changeover",
            PlannedFlag = true,
            StartTime = DateTimeOffset.UtcNow.AddHours(-6),
            EndTime = DateTimeOffset.UtcNow.AddHours(-5).AddMinutes(-10),
            DurationMs = 3_000_000,
            EnteredBy = userOperator.Id,
            Status = EntityStatus.Active,
        };

        var dtBreakdown = new DowntimeRecord
        {
            TenantId = tenant.Id,
            MachineId = machines[1].Id,
            LineId = line1.Id,
            ProductionOrderId = order1.Id,
            DowntimeReasonCode = "BRK",
            DowntimeReasonText = "Demo unplanned stop",
            PlannedFlag = false,
            StartTime = DateTimeOffset.UtcNow.AddHours(-4),
            EndTime = DateTimeOffset.UtcNow.AddHours(-3).AddMinutes(-30),
            DurationMs = 1_800_000,
            EnteredBy = userSupervisor.Id,
            Status = EntityStatus.Active,
        };

        var dtLineStop = new DowntimeRecord
        {
            TenantId = tenant.Id,
            LineId = line2.Id,
            MachineId = machines[4].Id,
            ProductionOrderId = order2.Id,
            DowntimeReasonCode = "WAIT-MAT",
            DowntimeReasonText = "Demo material wait",
            PlannedFlag = false,
            StartTime = DateTimeOffset.UtcNow.AddHours(-2),
            DurationMs = null,
            EnteredBy = userOperator.Id,
            Status = EntityStatus.Active,
        };

        var dtHydFail = new DowntimeRecord
        {
            TenantId = tenant.Id,
            MachineId = machines[0].Id,
            LineId = line1.Id,
            ProductionOrderId = order1.Id,
            DowntimeReasonCode = "HYD-FAIL",
            DowntimeReasonText = "Hydraulic failure — main pump fault.",
            PlannedFlag = false,
            StartTime = DateTimeOffset.UtcNow.AddHours(-3),
            EndTime = null,
            DurationMs = null,
            EnteredBy = userOperator.Id,
            Status = EntityStatus.Active,
        };

        var dtVibration204 = new DowntimeRecord
        {
            TenantId = tenant.Id,
            MachineId = machines[7].Id,
            LineId = line2.Id,
            ProductionOrderId = order2.Id,
            DowntimeReasonCode = "VIBRATION",
            DowntimeReasonText = "Spindle vibration fault — line stopped.",
            PlannedFlag = false,
            StartTime = DateTimeOffset.UtcNow.AddHours(-1),
            EndTime = null,
            DurationMs = null,
            EnteredBy = userOperator3.Id,
            Status = EntityStatus.Active,
        };

        var periodStartShift = DateTimeOffset.UtcNow.AddHours(-8);
        var periodEndShift = DateTimeOffset.UtcNow;
        var oeeSnapshots = new[]
        {
            MakeOee(tenant.Id, site.Id, line1.Id, machines[0].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.72m, 0.85m, 0.96m, 0.59m),
            MakeOee(tenant.Id, site.Id, line1.Id, machines[1].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.94m, 0.89m, 0.99m, 0.83m),
            MakeOee(tenant.Id, site.Id, line1.Id, machines[2].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.88m, 0.91m, 0.94m, 0.75m),
            MakeOee(tenant.Id, site.Id, line1.Id, machines[3].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.00m, 0.00m, 1.00m, 0.00m),
            MakeOee(tenant.Id, site.Id, line2.Id, machines[4].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.76m, 0.88m, 0.97m, 0.65m),
            MakeOee(tenant.Id, site.Id, line2.Id, machines[5].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.91m, 0.86m, 0.98m, 0.77m),
            MakeOee(tenant.Id, site.Id, line2.Id, machines[6].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.00m, 0.00m, 1.00m, 0.00m),
            MakeOee(tenant.Id, site.Id, line2.Id, machines[7].Id, OeePeriodType.Shift, periodStartShift, periodEndShift, 0.45m, 0.70m, 0.99m, 0.31m),
            MakeOee(tenant.Id, site.Id, line1.Id, null, OeePeriodType.Day, UtcStartOfToday(), UtcStartOfToday().AddDays(1), 0.81m, 0.87m, 0.972m, 0.685m),
            MakeOee(tenant.Id, site.Id, line2.Id, null, OeePeriodType.Day, UtcStartOfToday(), UtcStartOfToday().AddDays(1), 0.63m, 0.76m, 0.985m, 0.472m),
            MakeOee(tenant.Id, site.Id, null, null, OeePeriodType.Day, UtcStartOfToday(), UtcStartOfToday().AddDays(1), 0.72m, 0.81m, 0.978m, 0.571m),
        };

        var kpiOeeLine = new KpiDefinition
        {
            TenantId = tenant.Id,
            Code = "OEE_LINE_AGG",
            Name = "Line OEE aggregate (placeholder)",
            FormulaExpression = "availability * performance * quality",
            ScopeType = "line",
            DataSourceType = "oee_snapshot",
            Status = EntityStatus.Active,
        };

        var kpiScrap = new KpiDefinition
        {
            TenantId = tenant.Id,
            Code = "SCRAP_RATE",
            Name = "Scrap rate (placeholder)",
            FormulaExpression = "scrap_qty / good_qty",
            ScopeType = "line",
            DataSourceType = "mes_declaration",
            Status = EntityStatus.Active,
        };

        var kpiResults = new[]
        {
            new KpiResult
            {
                TenantId = tenant.Id,
                KpiDefinitionId = kpiOeeLine.Id,
                ScopeType = "line",
                ScopeId = line1.Id,
                PeriodStart = UtcStartOfToday(),
                PeriodEnd = UtcStartOfToday().AddDays(1),
                Value = 0.752m,
                Status = EntityStatus.Active,
            },
            new KpiResult
            {
                TenantId = tenant.Id,
                KpiDefinitionId = kpiOeeLine.Id,
                ScopeType = "machine",
                ScopeId = machines[0].Id,
                PeriodStart = UtcStartOfToday(),
                PeriodEnd = UtcStartOfToday().AddDays(1),
                Value = 0.801m,
                Status = EntityStatus.Active,
            },
            new KpiResult
            {
                TenantId = tenant.Id,
                KpiDefinitionId = kpiScrap.Id,
                ScopeType = "line",
                ScopeId = line1.Id,
                PeriodStart = UtcStartOfToday(),
                PeriodEnd = UtcStartOfToday().AddDays(1),
                Value = 0.024m,
                Status = EntityStatus.Active,
            },
            new KpiResult
            {
                TenantId = tenant.Id,
                KpiDefinitionId = kpiScrap.Id,
                ScopeType = "production_order",
                ScopeId = order1.Id,
                PeriodStart = DateTimeOffset.UtcNow.AddDays(-2),
                PeriodEnd = DateTimeOffset.UtcNow,
                Value = 0.018m,
                Status = EntityStatus.Active,
            },
        };

        var maintWorkOrderRefId = Guid.Parse("00000000-0000-4000-8000-000000000001");

        var wfDefRecipe = MakeWfDef(tenant.Id, "recipe_approval", "Recipe publish approval");
        var wfDefQuality = MakeWfDef(tenant.Id, "quality_approval", "Quality check / disposition approval");
        var wfDefOrderClose = MakeWfDef(tenant.Id, "order_close_approval", "Production order close approval");
        var wfDefMaint = MakeWfDef(tenant.Id, "maintenance_close_approval", "Maintenance work order close (placeholder type)");

        var wfRecipeStep1 = MakeWfStep(tenant.Id, wfDefRecipe.Id, 10, ApplicationRoles.Quality, WorkflowApprovalMode.Sequential);
        var wfRecipeStep2 = MakeWfStep(tenant.Id, wfDefRecipe.Id, 20, ApplicationRoles.TenantAdmin, WorkflowApprovalMode.Sequential);
        var wfQualityStep1 = MakeWfStep(tenant.Id, wfDefQuality.Id, 10, ApplicationRoles.TenantAdmin, WorkflowApprovalMode.ParallelAny);
        var wfQualityStep2 = MakeWfStep(tenant.Id, wfDefQuality.Id, 20, ApplicationRoles.Quality, WorkflowApprovalMode.Sequential);
        var wfOrderStep1 = MakeWfStep(tenant.Id, wfDefOrderClose.Id, 10, ApplicationRoles.ProductionManager, WorkflowApprovalMode.Sequential);
        var wfOrderStep2 = MakeWfStep(tenant.Id, wfDefOrderClose.Id, 20, ApplicationRoles.TenantAdmin, WorkflowApprovalMode.Sequential);
        var wfMaintStep1 = MakeWfStep(tenant.Id, wfDefMaint.Id, 10, ApplicationRoles.Maintenance, WorkflowApprovalMode.Sequential);

        var wfInstRecipe = new WorkflowInstance
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefRecipe.Id,
            ReferenceType = "recipe",
            ReferenceId = recipeAsm.Id,
            WorkflowState = WorkflowInstanceState.InProgress,
            StartedAt = DateTimeOffset.UtcNow.AddDays(-1),
            Status = EntityStatus.Active,
        };

        var wfInstQuality = new WorkflowInstance
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefQuality.Id,
            ReferenceType = "quality_check",
            ReferenceId = qcLine.Id,
            WorkflowState = WorkflowInstanceState.InProgress,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-8),
            Status = EntityStatus.Active,
        };

        var wfInstOrder = new WorkflowInstance
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefOrderClose.Id,
            ReferenceType = "production_order",
            ReferenceId = order1.Id,
            WorkflowState = WorkflowInstanceState.Approved,
            StartedAt = DateTimeOffset.UtcNow.AddDays(-3),
            EndedAt = DateTimeOffset.UtcNow.AddHours(-2),
            Status = EntityStatus.Active,
        };

        var wfInstMaint = new WorkflowInstance
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefMaint.Id,
            ReferenceType = "maintenance_work_order",
            ReferenceId = maintWorkOrderRefId,
            WorkflowState = WorkflowInstanceState.Draft,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-1),
            Status = EntityStatus.Active,
        };

        var wfActOrderApprove = new WorkflowAction
        {
            TenantId = tenant.Id,
            WorkflowInstanceId = wfInstOrder.Id,
            WorkflowStepId = wfOrderStep1.Id,
            ActionType = WorkflowActionType.Approve,
            ActionBy = userSupervisor.Id,
            ActionTime = DateTimeOffset.UtcNow.AddHours(-3),
            Comment = "Approved to close work order (demo)",
            Status = EntityStatus.Active,
        };

        var wfActRecipeComment = new WorkflowAction
        {
            TenantId = tenant.Id,
            WorkflowInstanceId = wfInstRecipe.Id,
            WorkflowStepId = wfRecipeStep1.Id,
            ActionType = WorkflowActionType.Comment,
            ActionBy = userOperator.Id,
            ActionTime = DateTimeOffset.UtcNow.AddHours(-12),
            Comment = "Recipe parameters reviewed on shop floor (demo)",
            Status = EntityStatus.Active,
        };

        var wfActQualityApprove = new WorkflowAction
        {
            TenantId = tenant.Id,
            WorkflowInstanceId = wfInstQuality.Id,
            WorkflowStepId = wfQualityStep1.Id,
            ActionType = WorkflowActionType.Approve,
            ActionBy = userSupervisor.Id,
            ActionTime = DateTimeOffset.UtcNow.AddHours(-6),
            Status = EntityStatus.Active,
        };

        db.Add(tenant);
        db.Add(enterprise);
        db.Add(site);
        db.AddRange(line1, line2);
        db.AddRange(machines);
        db.AddRange(
            roleSuperAdmin,
            roleTenantAdmin,
            roleSiteAdmin,
            roleProductionManager,
            roleQuality,
            roleMaintenance,
            roleOperator);
        db.AddRange(userOperator, userOperator2, userOperator3, userEngineer1, userSupervisor, userDevAdmin);
        db.AddRange(roleAssignments);
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
        db.AddRange(recipeAsm, recipeCoat);
        db.AddRange(recipeParams);
        db.AddRange(order1, order2, order3, order4);
        db.AddRange(op1a, op1b, op2a, op2b, op3a, op4a);
        db.AddRange(assign1, assign2, assign3, assign4);
        db.AddRange(lot1, lot2, lot3);
        db.AddRange(matCons);
        db.AddRange(prodDecl);
        db.Add(scrap1);
        db.AddRange(invMoves);
        db.AddRange(shiftMorning, shiftAfternoon, shiftMorning2, shiftAfternoon2);
        db.AddRange(empAssign);
        db.AddRange(qcLine, qcFinal);
        db.AddRange(qualityMeasurements);
        db.Add(ncDemo);
        db.Add(dispRework);
        db.AddRange(alarmDrive, alarmSite, alarmLine2, alarmTemp102, alarmVib204, alarmHyd101);
        db.AddRange(dtChangeover, dtBreakdown, dtLineStop, dtHydFail, dtVibration204);
        db.AddRange(oeeSnapshots);
        db.AddRange(kpiOeeLine, kpiScrap);
        db.AddRange(kpiResults);
        db.AddRange(
            wfDefRecipe,
            wfDefQuality,
            wfDefOrderClose,
            wfDefMaint,
            wfRecipeStep1,
            wfRecipeStep2,
            wfQualityStep1,
            wfQualityStep2,
            wfOrderStep1,
            wfOrderStep2,
            wfMaintStep1,
            wfInstRecipe,
            wfInstQuality,
            wfInstOrder,
            wfInstMaint,
            wfActOrderApprove,
            wfActRecipeComment,
            wfActQualityApprove);

        var pmM101 = new PreventiveMaintenanceSchedule
        {
            TenantId = tenant.Id,
            MachineId = machines[0].Id,
            Name = "Monthly preventive maintenance",
            IntervalDays = 30,
            LastDoneAt = DateTimeOffset.UtcNow.AddDays(-45),
            NextDueAt = DateTimeOffset.UtcNow.AddDays(-15),
            OwnerRole = "maintenance",
            Status = EntityStatus.Active,
        };

        var pmM102 = new PreventiveMaintenanceSchedule
        {
            TenantId = tenant.Id,
            MachineId = machines[1].Id,
            Name = "Weekly lubrication check",
            IntervalDays = 7,
            LastDoneAt = DateTimeOffset.UtcNow.AddDays(-3),
            NextDueAt = DateTimeOffset.UtcNow.AddDays(4),
            OwnerRole = "maintenance",
            Status = EntityStatus.Active,
        };

        var pmM201 = new PreventiveMaintenanceSchedule
        {
            TenantId = tenant.Id,
            MachineId = machines[4].Id,
            Name = "Quarterly mold inspection",
            IntervalDays = 90,
            LastDoneAt = DateTimeOffset.UtcNow.AddDays(-92),
            NextDueAt = DateTimeOffset.UtcNow.AddDays(-2),
            OwnerRole = "mold",
            Status = EntityStatus.Active,
        };

        var pmM103 = new PreventiveMaintenanceSchedule
        {
            TenantId = tenant.Id,
            MachineId = machines[2].Id,
            Name = "Bi-weekly tooling check",
            IntervalDays = 14,
            LastDoneAt = DateTimeOffset.UtcNow.AddDays(-10),
            NextDueAt = DateTimeOffset.UtcNow.AddDays(4),
            OwnerRole = "maintenance",
            Status = EntityStatus.Active,
        };

        var pmM202 = new PreventiveMaintenanceSchedule
        {
            TenantId = tenant.Id,
            MachineId = machines[5].Id,
            Name = "Monthly filter change",
            IntervalDays = 30,
            LastDoneAt = DateTimeOffset.UtcNow.AddDays(-20),
            NextDueAt = DateTimeOffset.UtcNow.AddDays(10),
            OwnerRole = "maintenance",
            Status = EntityStatus.Active,
        };

        var pmM204 = new PreventiveMaintenanceSchedule
        {
            TenantId = tenant.Id,
            MachineId = machines[7].Id,
            Name = "Weekly vibration check",
            IntervalDays = 7,
            LastDoneAt = DateTimeOffset.UtcNow.AddDays(-8),
            NextDueAt = DateTimeOffset.UtcNow.AddDays(-1),
            OwnerRole = "maintenance",
            Status = EntityStatus.Active,
        };

        var wo1 = new MaintenanceWorkOrder
        {
            TenantId = tenant.Id,
            MachineId = machines[0].Id,
            LineId = line1.Id,
            WorkOrderNo = "WO-20240115-001",
            WorkOrderType = MaintenanceWorkOrderType.Corrective,
            Priority = MaintenancePriority.P1Critical,
            WorkOrderStatus = MaintenanceWorkOrderStatus.InProgress,
            Description = "Hydraulic oil leak on machine M-101. Oil found pooling under unit.",
            ReasonCode = "HYD-LEAK",
            AssignedRole = "maintenance",
            AssignedTo = userSupervisor.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddHours(-3),
            ReportedBy = userOperator.Id,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-2),
            Status = EntityStatus.Active,
        };

        var wo2 = new MaintenanceWorkOrder
        {
            TenantId = tenant.Id,
            MachineId = machines[4].Id,
            LineId = line2.Id,
            WorkOrderNo = "WO-20240115-002",
            WorkOrderType = MaintenanceWorkOrderType.Preventive,
            Priority = MaintenancePriority.P2High,
            WorkOrderStatus = MaintenanceWorkOrderStatus.Open,
            Description = "Quarterly mold inspection overdue. Schedule and perform full inspection.",
            ReasonCode = "PM-MOLD-INSPECT",
            AssignedRole = "mold",
            ReportedAt = DateTimeOffset.UtcNow.AddHours(-1),
            ReportedBy = userSupervisor.Id,
            Status = EntityStatus.Active,
        };

        var wo3 = new MaintenanceWorkOrder
        {
            TenantId = tenant.Id,
            MachineId = machines[1].Id,
            LineId = line1.Id,
            WorkOrderNo = "WO-20240114-001",
            WorkOrderType = MaintenanceWorkOrderType.Corrective,
            Priority = MaintenancePriority.P3Medium,
            WorkOrderStatus = MaintenanceWorkOrderStatus.Done,
            Description = "Conveyor belt tension adjustment. Belt was slipping under load.",
            ReasonCode = "MECH-BELT",
            AssignedRole = "maintenance",
            AssignedTo = userSupervisor.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddDays(-1),
            ReportedBy = userOperator.Id,
            StartedAt = DateTimeOffset.UtcNow.AddDays(-1).AddHours(1),
            CompletedAt = DateTimeOffset.UtcNow.AddDays(-1).AddHours(3),
            ClosingNotes = "Belt tension adjusted and tested. Running normally.",
            Status = EntityStatus.Active,
        };

        var wo4 = new MaintenanceWorkOrder
        {
            TenantId = tenant.Id,
            MachineId = machines[7].Id,
            LineId = line2.Id,
            WorkOrderNo = "WO-20240116-004",
            WorkOrderType = MaintenanceWorkOrderType.Corrective,
            Priority = MaintenancePriority.P1Critical,
            WorkOrderStatus = MaintenanceWorkOrderStatus.Open,
            Description = "Spindle vibration — corrective investigation and bearing check.",
            ReasonCode = "VIB-SPINDLE",
            AssignedRole = "maintenance",
            AssignedTo = userEngineer1.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddHours(-2),
            ReportedBy = userOperator3.Id,
            Status = EntityStatus.Active,
        };

        var wo5 = new MaintenanceWorkOrder
        {
            TenantId = tenant.Id,
            MachineId = machines[2].Id,
            LineId = line1.Id,
            WorkOrderNo = "WO-20240116-005",
            WorkOrderType = MaintenanceWorkOrderType.MoldChange,
            Priority = MaintenancePriority.P2High,
            WorkOrderStatus = MaintenanceWorkOrderStatus.Open,
            Description = "Mold change for WO-DEMO-1003 (SKU-COVER-03).",
            ReasonCode = "MOLD-CHANGE",
            AssignedRole = "maintenance",
            ReportedAt = DateTimeOffset.UtcNow.AddMinutes(-90),
            ReportedBy = userSupervisor.Id,
            Status = EntityStatus.Active,
        };

        var wo6 = new MaintenanceWorkOrder
        {
            TenantId = tenant.Id,
            MachineId = machines[6].Id,
            LineId = line2.Id,
            WorkOrderNo = "WO-20240116-006",
            WorkOrderType = MaintenanceWorkOrderType.Preventive,
            Priority = MaintenancePriority.P4Low,
            WorkOrderStatus = MaintenanceWorkOrderStatus.Open,
            Description = "Monthly filter replacement on coating unit M-203.",
            ReasonCode = "PM-FILTER",
            AssignedRole = "maintenance",
            AssignedTo = userEngineer1.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddHours(-4),
            ReportedBy = userEngineer1.Id,
            Status = EntityStatus.Active,
        };

        var fault1 = new MachineFault
        {
            TenantId = tenant.Id,
            MachineId = machines[0].Id,
            MaintenanceWorkOrderId = wo1.Id,
            FaultCode = "HYD-001",
            Description = "Hydraulic pressure dropped below 180 bar. Oil leak detected at seal.",
            ReportedBy = userOperator.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddHours(-3).AddMinutes(-15),
            Resolved = false,
            Status = EntityStatus.Active,
        };

        var fault2 = new MachineFault
        {
            TenantId = tenant.Id,
            MachineId = machines[1].Id,
            MaintenanceWorkOrderId = wo3.Id,
            FaultCode = "MECH-003",
            Description = "Conveyor belt slipping detected on encoder feedback.",
            ReportedBy = userOperator.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddDays(-1).AddMinutes(-30),
            Resolved = true,
            ResolvedAt = DateTimeOffset.UtcNow.AddDays(-1).AddHours(3),
            Status = EntityStatus.Active,
        };

        var fault3 = new MachineFault
        {
            TenantId = tenant.Id,
            MachineId = machines[7].Id,
            MaintenanceWorkOrderId = wo4.Id,
            FaultCode = "VIB-001",
            Description = "Spindle vibration spectrum shows elevated harmonics.",
            ReportedBy = userOperator3.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddHours(-1).AddMinutes(-20),
            Resolved = false,
            Status = EntityStatus.Active,
        };

        var fault4 = new MachineFault
        {
            TenantId = tenant.Id,
            MachineId = machines[2].Id,
            MaintenanceWorkOrderId = null,
            FaultCode = "QUAL-002",
            Description = "Wall thickness out of specification on last cavity sample.",
            ReportedBy = userSupervisor.Id,
            ReportedAt = DateTimeOffset.UtcNow.AddHours(-5),
            Resolved = false,
            Status = EntityStatus.Active,
        };

        db.AddRange(pmM101, pmM102, pmM201, pmM103, pmM202, pmM204);
        db.AddRange(wo1, wo2, wo3, wo4, wo5, wo6);
        db.AddRange(fault1, fault2, fault3, fault4);

        var seedJob1 = new JobExecution
        {
            Id = DevSeedProductionIds.JobRunningOnM102,
            TenantId = tenant.Id,
            MachineId = machines[1].Id,
            LineId = machines[1].LineId,
            ProductionOrderId = order1.Id,
            RecipeId = recipeAsm.Id,
            ShiftId = shiftMorning.Id,
            OperatorId = userOperator.Id,
            JobNo = "JOB-20260427-001",
            ExecutionStatus = JobExecutionStatus.Running,
            PlannedQty = 500,
            ProducedQty = 213,
            ScrapQty = 4,
            DefectQty = 0,
            ActualStartAt = DateTimeOffset.UtcNow.AddHours(-6),
            Status = EntityStatus.Active,
        };

        var seedJob2 = new JobExecution
        {
            TenantId = tenant.Id,
            MachineId = machines[5].Id,
            LineId = machines[5].LineId,
            ProductionOrderId = order2.Id,
            RecipeId = recipeCoat.Id,
            ShiftId = shiftMorning2.Id,
            OperatorId = userOperator3.Id,
            JobNo = "JOB-20260427-002",
            ExecutionStatus = JobExecutionStatus.Running,
            PlannedQty = 300,
            ProducedQty = 89,
            ScrapQty = 1,
            DefectQty = 0,
            ActualStartAt = DateTimeOffset.UtcNow.AddHours(-4),
            Status = EntityStatus.Active,
        };

        var seedJob3 = new JobExecution
        {
            TenantId = tenant.Id,
            MachineId = machines[0].Id,
            LineId = machines[0].LineId,
            ProductionOrderId = order1.Id,
            RecipeId = recipeAsm.Id,
            ShiftId = shiftMorning.Id,
            OperatorId = userOperator.Id,
            JobNo = "JOB-20260427-003",
            ExecutionStatus = JobExecutionStatus.Paused,
            PauseReason = JobPauseReason.MachineBreakdown,
            PlannedQty = 400,
            ProducedQty = 67,
            ScrapQty = 12,
            DefectQty = 0,
            ActualStartAt = DateTimeOffset.UtcNow.AddHours(-8),
            PausedAt = DateTimeOffset.UtcNow.AddHours(-3),
            Status = EntityStatus.Active,
        };

        var seedJob4 = new JobExecution
        {
            TenantId = tenant.Id,
            MachineId = machines[4].Id,
            LineId = machines[4].LineId,
            ProductionOrderId = order2.Id,
            RecipeId = recipeCoat.Id,
            JobNo = "JOB-20260427-004",
            ExecutionStatus = JobExecutionStatus.Queued,
            PlannedQty = 200,
            ProducedQty = 0,
            ScrapQty = 0,
            DefectQty = 0,
            PlannedStartAt = DateTimeOffset.UtcNow.AddHours(2),
            Status = EntityStatus.Active,
        };

        var seedJob5 = new JobExecution
        {
            TenantId = tenant.Id,
            MachineId = machines[2].Id,
            LineId = machines[2].LineId,
            ProductionOrderId = order3.Id,
            RecipeId = recipeAsm.Id,
            ShiftId = shiftMorning.Id,
            OperatorId = userOperator2.Id,
            JobNo = "JOB-20260427-005",
            ExecutionStatus = JobExecutionStatus.Running,
            PlannedQty = 600,
            ProducedQty = 178,
            ScrapQty = 15,
            DefectQty = 0,
            ActualStartAt = DateTimeOffset.UtcNow.AddHours(-5),
            Status = EntityStatus.Active,
        };

        var seedJob6 = new JobExecution
        {
            TenantId = tenant.Id,
            MachineId = machines[3].Id,
            LineId = machines[3].LineId,
            ProductionOrderId = order1.Id,
            RecipeId = recipeAsm.Id,
            ShiftId = shiftMorning.Id,
            OperatorId = userOperator.Id,
            JobNo = "JOB-20260426-006",
            ExecutionStatus = JobExecutionStatus.Done,
            PlannedQty = 300,
            ProducedQty = 298,
            ScrapQty = 2,
            DefectQty = 0,
            ActualStartAt = DateTimeOffset.UtcNow.AddDays(-1).AddHours(-8),
            ActualEndAt = DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2),
            Status = EntityStatus.Active,
        };

        var seedJob7 = new JobExecution
        {
            TenantId = tenant.Id,
            MachineId = machines[6].Id,
            LineId = machines[6].LineId,
            ProductionOrderId = order2.Id,
            RecipeId = recipeCoat.Id,
            ShiftId = shiftMorning2.Id,
            OperatorId = userOperator3.Id,
            JobNo = "JOB-20260427-007",
            ExecutionStatus = JobExecutionStatus.Setup,
            PlannedQty = 150,
            ProducedQty = 0,
            ScrapQty = 0,
            DefectQty = 0,
            PlannedStartAt = DateTimeOffset.UtcNow.AddHours(1),
            Status = EntityStatus.Active,
        };

        var seedDecl1 = new OperatorDeclaration
        {
            TenantId = tenant.Id,
            JobExecutionId = seedJob1.Id,
            MachineId = machines[1].Id,
            OperatorId = userOperator.Id,
            DeclaredQty = 100,
            ScrapQty = 2,
            DefectQty = 0,
            DeclaredAt = DateTimeOffset.UtcNow.AddHours(-4),
            Status = EntityStatus.Active,
        };

        var seedDecl2 = new OperatorDeclaration
        {
            TenantId = tenant.Id,
            JobExecutionId = seedJob1.Id,
            MachineId = machines[1].Id,
            OperatorId = userOperator.Id,
            DeclaredQty = 113,
            ScrapQty = 2,
            DefectQty = 0,
            DeclaredAt = DateTimeOffset.UtcNow.AddHours(-2),
            Status = EntityStatus.Active,
        };

        var seedDeclJob2a = new OperatorDeclaration
        {
            TenantId = tenant.Id,
            JobExecutionId = seedJob2.Id,
            MachineId = machines[5].Id,
            OperatorId = userOperator3.Id,
            DeclaredQty = 45,
            ScrapQty = 0,
            DefectQty = 0,
            DeclaredAt = DateTimeOffset.UtcNow.AddHours(-3),
            Status = EntityStatus.Active,
        };

        var seedDeclJob2b = new OperatorDeclaration
        {
            TenantId = tenant.Id,
            JobExecutionId = seedJob2.Id,
            MachineId = machines[5].Id,
            OperatorId = userOperator3.Id,
            DeclaredQty = 44,
            ScrapQty = 1,
            DefectQty = 0,
            DeclaredAt = DateTimeOffset.UtcNow.AddHours(-1),
            Status = EntityStatus.Active,
        };

        var seedDeclJob5a = new OperatorDeclaration
        {
            TenantId = tenant.Id,
            JobExecutionId = seedJob5.Id,
            MachineId = machines[2].Id,
            OperatorId = userOperator2.Id,
            DeclaredQty = 95,
            ScrapQty = 8,
            DefectQty = 0,
            DeclaredAt = DateTimeOffset.UtcNow.AddHours(-4),
            Status = EntityStatus.Active,
        };

        var seedDeclJob5b = new OperatorDeclaration
        {
            TenantId = tenant.Id,
            JobExecutionId = seedJob5.Id,
            MachineId = machines[2].Id,
            OperatorId = userOperator2.Id,
            DeclaredQty = 83,
            ScrapQty = 7,
            DefectQty = 0,
            DeclaredAt = DateTimeOffset.UtcNow.AddHours(-2),
            Status = EntityStatus.Active,
        };

        db.AddRange(seedJob1, seedJob2, seedJob3, seedJob4, seedJob5, seedJob6, seedJob7);
        db.AddRange(seedDecl1, seedDecl2, seedDeclJob2a, seedDeclJob2b, seedDeclJob5a, seedDeclJob5b);

        var mondayThisWeekUtc = StartOfWeekMondayUtc(DateTime.UtcNow);
        var fridayThisWeekDate = mondayThisWeekUtc.UtcDateTime.Date.AddDays(4);
        var plan1PlannedStart = mondayThisWeekUtc.AddHours(6);
        var plan1PlannedEnd = new DateTimeOffset(fridayThisWeekDate, TimeSpan.Zero).AddHours(22);

        var nextMondayUtc = mondayThisWeekUtc.AddDays(7);
        var plan2PlannedStart = nextMondayUtc.AddHours(6);
        var plan2PlannedEnd = nextMondayUtc.AddDays(2).AddHours(22);

        var lastWeekMonday = StartOfWeekMondayUtc(DateTime.UtcNow).AddDays(-7);
        var lastWeekFriday = lastWeekMonday.UtcDateTime.Date.AddDays(4);
        var plan3PlannedStart = lastWeekMonday.AddHours(6);
        var plan3PlannedEnd = new DateTimeOffset(lastWeekFriday, TimeSpan.Zero).AddHours(22);

        var utcToday = UtcStartOfToday();
        var utcTomorrow = utcToday.AddDays(1);

        var plan1 = new ProductionPlan
        {
            TenantId = tenant.Id,
            PlanNo = "PLAN-20260427-001",
            Name = "Week 19 Injection Run",
            PlanStatus = PlanStatus.Approved,
            Priority = PlanPriority.High,
            PlannedStartAt = plan1PlannedStart,
            PlannedEndAt = plan1PlannedEnd,
            SiteId = site.Id,
            LineId = line1.Id,
            CreatedByUserId = userSupervisor.Id,
            ApprovedByUserId = userSupervisor.Id,
            ApprovedAt = DateTimeOffset.UtcNow.AddDays(-2),
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var plan2 = new ProductionPlan
        {
            TenantId = tenant.Id,
            PlanNo = "PLAN-20260427-002",
            Name = "Week 20 Prep Run",
            PlanStatus = PlanStatus.Draft,
            Priority = PlanPriority.Medium,
            PlannedStartAt = plan2PlannedStart,
            PlannedEndAt = plan2PlannedEnd,
            SiteId = site.Id,
            LineId = line1.Id,
            CreatedByUserId = userSupervisor.Id,
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var plan3 = new ProductionPlan
        {
            TenantId = tenant.Id,
            PlanNo = "PLAN-20260420-003",
            Name = "Previous Week Run",
            PlanStatus = PlanStatus.Done,
            Priority = PlanPriority.Medium,
            PlannedStartAt = plan3PlannedStart,
            PlannedEndAt = plan3PlannedEnd,
            SiteId = site.Id,
            LineId = line1.Id,
            CreatedByUserId = userSupervisor.Id,
            ApprovedByUserId = userSupervisor.Id,
            ApprovedAt = DateTimeOffset.UtcNow.AddDays(-9),
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var slot1 = new PlanSlot
        {
            TenantId = tenant.Id,
            ProductionPlanId = plan1.Id,
            MachineId = machines[1].Id,
            ProductionOrderId = order1.Id,
            JobExecutionId = seedJob1.Id,
            SlotNo = "SLOT-001",
            SlotStatus = PlanSlotStatus.Running,
            Priority = PlanPriority.High,
            PlannedQty = 500,
            PlannedStartAt = utcToday.AddHours(6),
            PlannedEndAt = utcToday.AddHours(14),
            SortOrder = 1,
            SetupTimeMinutes = 30,
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var slot2 = new PlanSlot
        {
            TenantId = tenant.Id,
            ProductionPlanId = plan1.Id,
            MachineId = machines[2].Id,
            ProductionOrderId = order3.Id,
            JobExecutionId = seedJob5.Id,
            SlotNo = "SLOT-002",
            SlotStatus = PlanSlotStatus.Running,
            Priority = PlanPriority.High,
            PlannedQty = 600,
            PlannedStartAt = utcToday.AddHours(7),
            PlannedEndAt = utcToday.AddHours(15),
            SortOrder = 2,
            SetupTimeMinutes = 25,
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var slot3 = new PlanSlot
        {
            TenantId = tenant.Id,
            ProductionPlanId = plan1.Id,
            MachineId = machines[3].Id,
            ProductionOrderId = order1.Id,
            SlotNo = "SLOT-003",
            SlotStatus = PlanSlotStatus.Scheduled,
            Priority = PlanPriority.High,
            PlannedQty = 250,
            PlannedStartAt = utcTomorrow.AddHours(6),
            PlannedEndAt = utcTomorrow.AddHours(14),
            SortOrder = 3,
            SetupTimeMinutes = 20,
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var slot4 = new PlanSlot
        {
            TenantId = tenant.Id,
            ProductionPlanId = plan1.Id,
            MachineId = machines[0].Id,
            ProductionOrderId = order1.Id,
            SlotNo = "SLOT-004",
            SlotStatus = PlanSlotStatus.Skipped,
            Priority = PlanPriority.High,
            PlannedQty = 400,
            PlannedStartAt = utcToday.AddHours(10),
            PlannedEndAt = utcToday.AddHours(18),
            SortOrder = 4,
            SetupTimeMinutes = 45,
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var slot5 = new PlanSlot
        {
            TenantId = tenant.Id,
            ProductionPlanId = plan1.Id,
            MachineId = machines[5].Id,
            ProductionOrderId = order2.Id,
            JobExecutionId = seedJob2.Id,
            SlotNo = "SLOT-005",
            SlotStatus = PlanSlotStatus.Running,
            Priority = PlanPriority.High,
            PlannedQty = 300,
            PlannedStartAt = utcToday.AddHours(8),
            PlannedEndAt = utcToday.AddHours(16),
            SortOrder = 5,
            SetupTimeMinutes = 20,
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        var slot6 = new PlanSlot
        {
            TenantId = tenant.Id,
            ProductionPlanId = plan1.Id,
            MachineId = machines[4].Id,
            ProductionOrderId = order2.Id,
            SlotNo = "SLOT-006",
            SlotStatus = PlanSlotStatus.Scheduled,
            Priority = PlanPriority.High,
            PlannedQty = 200,
            PlannedStartAt = utcTomorrow.AddHours(10),
            PlannedEndAt = utcTomorrow.AddHours(16),
            SortOrder = 6,
            SetupTimeMinutes = 30,
            Status = EntityStatus.Active,
            CreatedBy = userSupervisor.Id,
            UpdatedBy = userSupervisor.Id,
        };

        db.AddRange(plan1, plan2, plan3);
        db.AddRange(slot1, slot2, slot3, slot4, slot5, slot6);

        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Dev seed completed.");
    }

    // UTC midnight (offset 0); DateTimeOffset.UtcNow.Date would be Unspecified→local offset and breaks Npgsql timestamptz.
    private static DateTimeOffset UtcStartOfToday() =>
        new DateTimeOffset(DateTimeOffset.UtcNow.UtcDateTime.Date, TimeSpan.Zero);

    private static DateTimeOffset StartOfWeekMondayUtc(DateTime utcNow)
    {
        var date = utcNow.Date;
        var day = utcNow.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)utcNow.DayOfWeek;
        var daysSinceMonday = day - (int)DayOfWeek.Monday;
        var monday = date.AddDays(-daysSinceMonday);
        return new DateTimeOffset(monday, TimeSpan.Zero);
    }

    /// <summary>Creates an active <see cref="Role"/> row for the tenant.</summary>
    private static Role MakeRole(Guid tenantId, string code, string name) =>
        new()
        {
            TenantId = tenantId,
            Code = code,
            Name = name,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a tenant-scoped <see cref="UserRoleAssignment"/>.</summary>
    private static UserRoleAssignment MakeRoleAssignment(
        Guid tenantId,
        Guid userId,
        Guid roleId,
        ScopeType scopeType,
        Guid scopeId) =>
        new()
        {
            TenantId = tenantId,
            UserAccountId = userId,
            RoleId = roleId,
            ScopeType = scopeType,
            ScopeId = scopeId,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="ConnectorDefinition"/> with common defaults.</summary>
    private static ConnectorDefinition MakeConnDef(
        Guid tenantId,
        string code,
        string name,
        string category,
        ConnectorDirection direction,
        bool read,
        bool write) =>
        new()
        {
            TenantId = tenantId,
            Code = code,
            Name = name,
            Category = category,
            Direction = direction,
            SupportsRead = read,
            SupportsWrite = write,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates an active <see cref="Recipe"/>.</summary>
    private static Recipe MakeRecipe(Guid tenantId, string code, string name, int version, string? description) =>
        new()
        {
            TenantId = tenantId,
            Code = code,
            Name = name,
            Version = version,
            Description = description,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="RecipeParameter"/> bound to a recipe.</summary>
    private static RecipeParameter MakeRecipeParam(
        Guid recipeId,
        string code,
        string name,
        string dataType,
        string unit,
        string defaultValue,
        string minValue,
        string maxValue,
        int sortOrder) =>
        new()
        {
            RecipeId = recipeId,
            Code = code,
            Name = name,
            DataType = dataType,
            Unit = unit,
            DefaultValue = defaultValue,
            MinValue = minValue,
            MaxValue = maxValue,
            SortOrder = sortOrder,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a demo <see cref="ProductionOrder"/>.</summary>
    private static ProductionOrder MakeOrder(
        Guid tenantId,
        Guid siteId,
        Guid lineId,
        string orderNo,
        string productCode,
        int planned,
        int completed,
        ProductionOrderStatus status,
        DateTimeOffset? plannedStart,
        DateTimeOffset? plannedEnd,
        string? sourceRef)
    {
        var o = new ProductionOrder
        {
            TenantId = tenantId,
            SiteId = siteId,
            LineId = lineId,
            OrderNo = orderNo,
            ProductCode = productCode,
            QuantityPlanned = planned,
            QuantityCompleted = completed,
            OrderStatus = status,
            Status = EntityStatus.Active,
            PlannedStartAt = plannedStart,
            PlannedEndAt = plannedEnd,
            SourceSystem = "demo",
            SourceReference = sourceRef,
        };
        return o;
    }

    /// <summary>Creates a <see cref="ProductionOperation"/>.</summary>
    private static ProductionOperation MakeOp(
        Guid tenantId,
        Guid orderId,
        int sequenceNo,
        string code,
        string name,
        Guid lineId,
        Guid? machineId,
        ProductionOperationStatus opStatus,
        int planned,
        int completed) =>
        new()
        {
            TenantId = tenantId,
            ProductionOrderId = orderId,
            SequenceNo = sequenceNo,
            Code = code,
            Name = name,
            LineId = lineId,
            MachineId = machineId,
            OperationStatus = opStatus,
            QuantityPlanned = planned,
            QuantityCompleted = completed,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates an <see cref="OrderRecipeAssignment"/>.</summary>
    private static OrderRecipeAssignment MakeOrderRecipe(
        Guid tenantId,
        Guid orderId,
        Guid recipeId,
        int recipeVersion,
        DateTimeOffset assignedAt) =>
        new()
        {
            TenantId = tenantId,
            ProductionOrderId = orderId,
            RecipeId = recipeId,
            RecipeVersionAssigned = recipeVersion,
            AssignedAt = assignedAt,
            IsPrimary = true,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates an active <see cref="LotBatch"/>.</summary>
    private static LotBatch MakeLot(
        Guid tenantId,
        Guid orderId,
        Guid lineId,
        Guid machineId,
        string lotNo,
        int targetQty,
        int goodQty) =>
        new()
        {
            TenantId = tenantId,
            ProductionOrderId = orderId,
            LineId = lineId,
            MachineId = machineId,
            LotNo = lotNo,
            LotStatus = LotBatchStatus.Active,
            TargetQuantity = targetQty,
            QuantityGood = goodQty,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-6),
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="MaterialConsumption"/> row.</summary>
    private static MaterialConsumption MakeMat(
        Guid tenantId,
        Guid lotId,
        string materialCode,
        string materialName,
        decimal quantity,
        string unit,
        DateTimeOffset consumedAt) =>
        new()
        {
            TenantId = tenantId,
            LotBatchId = lotId,
            MaterialCode = materialCode,
            MaterialName = materialName,
            Quantity = quantity,
            Unit = unit,
            ConsumedAt = consumedAt,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="ProductionDeclaration"/>.</summary>
    private static ProductionDeclaration MakeProdDecl(
        Guid tenantId,
        Guid lotId,
        Guid lineId,
        Guid machineId,
        int goodQty,
        DateTimeOffset declaredAt,
        string? notes) =>
        new()
        {
            TenantId = tenantId,
            LotBatchId = lotId,
            GoodQuantity = goodQty,
            DeclaredAt = declaredAt,
            LineId = lineId,
            MachineId = machineId,
            Notes = notes,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="Shift"/> for a line and calendar day.</summary>
    private static Shift MakeShift(
        Guid tenantId,
        Guid siteId,
        Guid lineId,
        string code,
        string name,
        DateOnly shiftDate,
        DateTime dayStartUtc,
        int startHour,
        int endHour,
        WorkShiftLifecycle lifecycle,
        double? actualStartHourOffset)
    {
        var plannedStart = new DateTimeOffset(dayStartUtc.AddHours(startHour));
        var plannedEnd = new DateTimeOffset(dayStartUtc.AddHours(endHour));
        return new Shift
        {
            TenantId = tenantId,
            SiteId = siteId,
            LineId = lineId,
            Code = code,
            Name = name,
            ShiftDate = shiftDate,
            PlannedStartAt = plannedStart,
            PlannedEndAt = plannedEnd,
            ActualStartAt = actualStartHourOffset is null
                ? null
                : new DateTimeOffset(dayStartUtc.AddHours(actualStartHourOffset.Value)),
            Lifecycle = lifecycle,
            Status = EntityStatus.Active,
        };
    }

    /// <summary>Creates an <see cref="EmployeeAssignment"/> for a shift.</summary>
    private static EmployeeAssignment MakeEmpAssign(
        Guid tenantId,
        Guid userId,
        Guid shiftId,
        Guid lineId,
        Guid? machineId,
        DateTimeOffset from,
        DateTimeOffset? to,
        string role) =>
        new()
        {
            TenantId = tenantId,
            UserAccountId = userId,
            ShiftId = shiftId,
            LineId = lineId,
            MachineId = machineId,
            AssignedFrom = from,
            AssignedTo = to,
            AssignmentRole = role,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="QualityMeasurement"/>.</summary>
    private static QualityMeasurement MakeMeasurement(
        Guid tenantId,
        Guid checkId,
        string parameterCode,
        string measured,
        string? target,
        string? min,
        string? max,
        QualityMeasurementResult result) =>
        new()
        {
            TenantId = tenantId,
            QualityCheckId = checkId,
            ParameterCode = parameterCode,
            MeasuredValue = measured,
            TargetValue = target,
            MinValue = min,
            MaxValue = max,
            Result = result,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates an <see cref="OeeSnapshot"/> row.</summary>
    private static OeeSnapshot MakeOee(
        Guid tenantId,
        Guid siteId,
        Guid? lineId,
        Guid? machineId,
        OeePeriodType periodType,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        decimal availability,
        decimal performance,
        decimal quality,
        decimal oee) =>
        new()
        {
            TenantId = tenantId,
            SiteId = siteId,
            LineId = lineId,
            MachineId = machineId,
            PeriodType = periodType,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Availability = availability,
            Performance = performance,
            Quality = quality,
            OeeValue = oee,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="WorkflowDefinition"/>.</summary>
    private static WorkflowDefinition MakeWfDef(Guid tenantId, string workflowType, string name) =>
        new()
        {
            TenantId = tenantId,
            WorkflowType = workflowType,
            Name = name,
            Status = EntityStatus.Active,
        };

    /// <summary>Creates a <see cref="WorkflowStep"/>.</summary>
    private static WorkflowStep MakeWfStep(
        Guid tenantId,
        Guid definitionId,
        int sequenceNo,
        string roleCode,
        WorkflowApprovalMode mode) =>
        new()
        {
            TenantId = tenantId,
            WorkflowDefinitionId = definitionId,
            SequenceNo = sequenceNo,
            RoleCode = roleCode,
            ApprovalMode = mode,
            Status = EntityStatus.Active,
        };

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
