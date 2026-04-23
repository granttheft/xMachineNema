using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Auth.Security;
using XMachine.Module.Commercial.Domain;
using XMachine.Module.Engineering.Domain;
using XMachine.Module.Eventing.Domain;
using XMachine.Module.Integration.Domain;
using XMachine.Module.MES.Domain;
using XMachine.Module.Platform.Domain;
using XMachine.Module.Quality.Domain;
using XMachine.Module.Workflow.Domain;
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

        var roleSuperAdmin = new Role
        {
            TenantId = tenant.Id,
            Code = ApplicationRoles.SuperAdmin,
            Name = "Super admin",
            Status = EntityStatus.Active,
        };

        var roleTenantAdmin = new Role
        {
            TenantId = tenant.Id,
            Code = ApplicationRoles.TenantAdmin,
            Name = "Tenant admin",
            Status = EntityStatus.Active,
        };

        var roleSiteAdmin = new Role
        {
            TenantId = tenant.Id,
            Code = ApplicationRoles.SiteAdmin,
            Name = "Site admin",
            Status = EntityStatus.Active,
        };

        var roleProductionManager = new Role
        {
            TenantId = tenant.Id,
            Code = ApplicationRoles.ProductionManager,
            Name = "Production manager",
            Status = EntityStatus.Active,
        };

        var roleQuality = new Role
        {
            TenantId = tenant.Id,
            Code = ApplicationRoles.Quality,
            Name = "Quality",
            Status = EntityStatus.Active,
        };

        var roleMaintenance = new Role
        {
            TenantId = tenant.Id,
            Code = ApplicationRoles.Maintenance,
            Name = "Maintenance",
            Status = EntityStatus.Active,
        };

        var roleOperator = new Role
        {
            TenantId = tenant.Id,
            Code = ApplicationRoles.Operator,
            Name = "Operator",
            Status = EntityStatus.Active,
        };

        var userOperator = new UserAccount
        {
            TenantId = tenant.Id,
            Username = "operator1",
            DisplayName = "Demo Operator",
            Email = "operator1@demo.local",
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
            new UserRoleAssignment
            {
                TenantId = tenant.Id,
                UserAccountId = userDevAdmin.Id,
                RoleId = roleSuperAdmin.Id,
                ScopeType = ScopeType.Tenant,
                ScopeId = tenant.Id,
                Status = EntityStatus.Active,
            },
            new UserRoleAssignment
            {
                TenantId = tenant.Id,
                UserAccountId = userSupervisor.Id,
                RoleId = roleTenantAdmin.Id,
                ScopeType = ScopeType.Tenant,
                ScopeId = tenant.Id,
                Status = EntityStatus.Active,
            },
            new UserRoleAssignment
            {
                TenantId = tenant.Id,
                UserAccountId = userSupervisor.Id,
                RoleId = roleProductionManager.Id,
                ScopeType = ScopeType.Tenant,
                ScopeId = tenant.Id,
                Status = EntityStatus.Active,
            },
            new UserRoleAssignment
            {
                TenantId = tenant.Id,
                UserAccountId = userOperator.Id,
                RoleId = roleOperator.Id,
                ScopeType = ScopeType.Tenant,
                ScopeId = tenant.Id,
                Status = EntityStatus.Active,
            },
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

        var recipeAsm = new Recipe
        {
            TenantId = tenant.Id,
            Code = "ASM-A",
            Name = "Assembly recipe A",
            Version = 1,
            Description = "Demo assembly parameters",
            Status = EntityStatus.Active,
        };

        var recipeCoat = new Recipe
        {
            TenantId = tenant.Id,
            Code = "COAT-B",
            Name = "Coating recipe B",
            Version = 1,
            Description = "Demo coating parameters",
            Status = EntityStatus.Active,
        };

        var recipeParams = new[]
        {
            new RecipeParameter
            {
                RecipeId = recipeAsm.Id,
                Code = "SPINDLE_RPM",
                Name = "Target spindle rpm",
                DataType = "number",
                Unit = "rpm",
                DefaultValue = "2400",
                MinValue = "0",
                MaxValue = "8000",
                SortOrder = 10,
                Status = EntityStatus.Active,
            },
            new RecipeParameter
            {
                RecipeId = recipeAsm.Id,
                Code = "CYCLE_TIME_S",
                Name = "Nominal cycle time",
                DataType = "number",
                Unit = "s",
                DefaultValue = "45",
                MinValue = "10",
                MaxValue = "300",
                SortOrder = 20,
                Status = EntityStatus.Active,
            },
            new RecipeParameter
            {
                RecipeId = recipeAsm.Id,
                Code = "TORQUE_NM",
                Name = "Fastening torque",
                DataType = "number",
                Unit = "Nm",
                DefaultValue = "35",
                MinValue = "5",
                MaxValue = "120",
                SortOrder = 30,
                Status = EntityStatus.Active,
            },
            new RecipeParameter
            {
                RecipeId = recipeCoat.Id,
                Code = "OVEN_TEMP_C",
                Name = "Oven temperature",
                DataType = "number",
                Unit = "C",
                DefaultValue = "180",
                MinValue = "120",
                MaxValue = "220",
                SortOrder = 10,
                Status = EntityStatus.Active,
            },
            new RecipeParameter
            {
                RecipeId = recipeCoat.Id,
                Code = "CURE_TIME_S",
                Name = "Cure time",
                DataType = "number",
                Unit = "s",
                DefaultValue = "900",
                MinValue = "60",
                MaxValue = "7200",
                SortOrder = 20,
                Status = EntityStatus.Active,
            },
        };

        var order1 = new ProductionOrder
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line1.Id,
            OrderNo = "WO-DEMO-1001",
            ProductCode = "SKU-BRACKET-01",
            QuantityPlanned = 500,
            QuantityCompleted = 120,
            OrderStatus = ProductionOrderStatus.InProgress,
            Status = EntityStatus.Active,
            PlannedStartAt = DateTimeOffset.UtcNow.AddDays(-2),
            PlannedEndAt = DateTimeOffset.UtcNow.AddDays(3),
            SourceSystem = "demo",
            SourceReference = "ERP-WO-1001",
        };

        var order2 = new ProductionOrder
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line2.Id,
            OrderNo = "WO-DEMO-1002",
            ProductCode = "SKU-PANEL-02",
            QuantityPlanned = 200,
            QuantityCompleted = 25,
            OrderStatus = ProductionOrderStatus.Released,
            Status = EntityStatus.Active,
            PlannedStartAt = DateTimeOffset.UtcNow.AddDays(-1),
            SourceSystem = "demo",
        };

        var op1a = new ProductionOperation
        {
            TenantId = tenant.Id,
            ProductionOrderId = order1.Id,
            SequenceNo = 10,
            Code = "ASM",
            Name = "Assemble",
            LineId = line1.Id,
            MachineId = machines[0].Id,
            OperationStatus = ProductionOperationStatus.Running,
            QuantityPlanned = 500,
            QuantityCompleted = 120,
            Status = EntityStatus.Active,
        };

        var op1b = new ProductionOperation
        {
            TenantId = tenant.Id,
            ProductionOrderId = order1.Id,
            SequenceNo = 20,
            Code = "PACK",
            Name = "Pack",
            LineId = line1.Id,
            OperationStatus = ProductionOperationStatus.Pending,
            QuantityPlanned = 500,
            QuantityCompleted = 0,
            Status = EntityStatus.Active,
        };

        var op2a = new ProductionOperation
        {
            TenantId = tenant.Id,
            ProductionOrderId = order2.Id,
            SequenceNo = 10,
            Code = "COAT",
            Name = "Coat",
            LineId = line2.Id,
            MachineId = machines[2].Id,
            OperationStatus = ProductionOperationStatus.Running,
            QuantityPlanned = 200,
            QuantityCompleted = 25,
            Status = EntityStatus.Active,
        };

        var op2b = new ProductionOperation
        {
            TenantId = tenant.Id,
            ProductionOrderId = order2.Id,
            SequenceNo = 20,
            Code = "QC_VISUAL",
            Name = "Visual check",
            LineId = line2.Id,
            OperationStatus = ProductionOperationStatus.Pending,
            QuantityPlanned = 200,
            QuantityCompleted = 0,
            Status = EntityStatus.Active,
        };

        var assign1 = new OrderRecipeAssignment
        {
            TenantId = tenant.Id,
            ProductionOrderId = order1.Id,
            RecipeId = recipeAsm.Id,
            RecipeVersionAssigned = recipeAsm.Version,
            AssignedAt = DateTimeOffset.UtcNow.AddDays(-2),
            IsPrimary = true,
            Status = EntityStatus.Active,
        };

        var assign2 = new OrderRecipeAssignment
        {
            TenantId = tenant.Id,
            ProductionOrderId = order2.Id,
            RecipeId = recipeCoat.Id,
            RecipeVersionAssigned = recipeCoat.Version,
            AssignedAt = DateTimeOffset.UtcNow.AddDays(-1),
            IsPrimary = true,
            Status = EntityStatus.Active,
        };

        var lot1 = new LotBatch
        {
            TenantId = tenant.Id,
            ProductionOrderId = order1.Id,
            LineId = line1.Id,
            MachineId = machines[0].Id,
            LotNo = "LOT-DEMO-1001-A",
            LotStatus = LotBatchStatus.Active,
            TargetQuantity = 500,
            QuantityGood = 120,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-6),
            Status = EntityStatus.Active,
        };

        var lot2 = new LotBatch
        {
            TenantId = tenant.Id,
            ProductionOrderId = order2.Id,
            LineId = line2.Id,
            MachineId = machines[2].Id,
            LotNo = "LOT-DEMO-1002-A",
            LotStatus = LotBatchStatus.Active,
            TargetQuantity = 200,
            QuantityGood = 25,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-3),
            Status = EntityStatus.Active,
        };

        var matCons = new[]
        {
            new MaterialConsumption
            {
                TenantId = tenant.Id,
                LotBatchId = lot1.Id,
                MaterialCode = "RM-STEEL-01",
                MaterialName = "Steel blank",
                Quantity = 130,
                Unit = "ea",
                ConsumedAt = DateTimeOffset.UtcNow.AddHours(-5),
                Status = EntityStatus.Active,
            },
            new MaterialConsumption
            {
                TenantId = tenant.Id,
                LotBatchId = lot1.Id,
                MaterialCode = "RM-SCREW-M4",
                MaterialName = "Screw M4",
                Quantity = 520,
                Unit = "ea",
                ConsumedAt = DateTimeOffset.UtcNow.AddHours(-5),
                Status = EntityStatus.Active,
            },
            new MaterialConsumption
            {
                TenantId = tenant.Id,
                LotBatchId = lot2.Id,
                MaterialCode = "RM-PAINT-WHITE",
                MaterialName = "Paint white",
                Quantity = 12,
                Unit = "kg",
                ConsumedAt = DateTimeOffset.UtcNow.AddHours(-2),
                Status = EntityStatus.Active,
            },
        };

        var prodDecl = new[]
        {
            new ProductionDeclaration
            {
                TenantId = tenant.Id,
                LotBatchId = lot1.Id,
                GoodQuantity = 40,
                DeclaredAt = DateTimeOffset.UtcNow.AddHours(-4),
                LineId = line1.Id,
                MachineId = machines[0].Id,
                Notes = "Demo run 1",
                Status = EntityStatus.Active,
            },
            new ProductionDeclaration
            {
                TenantId = tenant.Id,
                LotBatchId = lot1.Id,
                GoodQuantity = 50,
                DeclaredAt = DateTimeOffset.UtcNow.AddHours(-3),
                LineId = line1.Id,
                MachineId = machines[0].Id,
                Status = EntityStatus.Active,
            },
            new ProductionDeclaration
            {
                TenantId = tenant.Id,
                LotBatchId = lot1.Id,
                GoodQuantity = 30,
                DeclaredAt = DateTimeOffset.UtcNow.AddHours(-2),
                LineId = line1.Id,
                MachineId = machines[1].Id,
                Status = EntityStatus.Active,
            },
            new ProductionDeclaration
            {
                TenantId = tenant.Id,
                LotBatchId = lot2.Id,
                GoodQuantity = 25,
                DeclaredAt = DateTimeOffset.UtcNow.AddHours(-1),
                LineId = line2.Id,
                MachineId = machines[2].Id,
                Status = EntityStatus.Active,
            },
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

        var shiftMorning = new Shift
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line1.Id,
            Code = "DAY-A",
            Name = "Line 1 day shift",
            ShiftDate = shiftDay,
            PlannedStartAt = new DateTimeOffset(dayStart.AddHours(6)),
            PlannedEndAt = new DateTimeOffset(dayStart.AddHours(14)),
            ActualStartAt = new DateTimeOffset(dayStart.AddHours(6).AddMinutes(5)),
            Lifecycle = WorkShiftLifecycle.Open,
            Status = EntityStatus.Active,
        };

        var shiftAfternoon = new Shift
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line2.Id,
            Code = "DAY-B",
            Name = "Line 2 day shift",
            ShiftDate = shiftDay,
            PlannedStartAt = new DateTimeOffset(dayStart.AddHours(14)),
            PlannedEndAt = new DateTimeOffset(dayStart.AddHours(22)),
            Lifecycle = WorkShiftLifecycle.Planned,
            Status = EntityStatus.Active,
        };

        var empAssign = new[]
        {
            new EmployeeAssignment
            {
                TenantId = tenant.Id,
                UserAccountId = userOperator.Id,
                ShiftId = shiftMorning.Id,
                LineId = line1.Id,
                AssignedFrom = shiftMorning.PlannedStartAt,
                AssignedTo = shiftMorning.PlannedEndAt,
                AssignmentRole = "operator",
                Status = EntityStatus.Active,
            },
            new EmployeeAssignment
            {
                TenantId = tenant.Id,
                UserAccountId = userSupervisor.Id,
                ShiftId = shiftMorning.Id,
                AssignedFrom = shiftMorning.PlannedStartAt,
                AssignedTo = shiftMorning.PlannedEndAt,
                AssignmentRole = "supervisor",
                Status = EntityStatus.Active,
            },
            new EmployeeAssignment
            {
                TenantId = tenant.Id,
                UserAccountId = userOperator.Id,
                ShiftId = shiftAfternoon.Id,
                LineId = line1.Id,
                MachineId = machines[0].Id,
                AssignedFrom = shiftAfternoon.PlannedStartAt,
                AssignmentRole = "operator",
                Status = EntityStatus.Active,
            },
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
            MachineId = machines[2].Id,
            CheckType = "final",
            CheckTime = DateTimeOffset.UtcNow.AddHours(-1),
            CheckStatus = QualityCheckStatus.Pending,
            Status = EntityStatus.Active,
        };

        var qualityMeasurements = new[]
        {
            new QualityMeasurement
            {
                TenantId = tenant.Id,
                QualityCheckId = qcLine.Id,
                ParameterCode = "LENGTH_MM",
                MeasuredValue = "100.02",
                TargetValue = "100.00",
                MinValue = "99.90",
                MaxValue = "100.10",
                Result = QualityMeasurementResult.Pass,
                Status = EntityStatus.Active,
            },
            new QualityMeasurement
            {
                TenantId = tenant.Id,
                QualityCheckId = qcLine.Id,
                ParameterCode = "WIDTH_MM",
                MeasuredValue = "50.08",
                TargetValue = "50.00",
                MinValue = "49.85",
                MaxValue = "50.15",
                Result = QualityMeasurementResult.Warning,
                Status = EntityStatus.Active,
            },
            new QualityMeasurement
            {
                TenantId = tenant.Id,
                QualityCheckId = qcLine.Id,
                ParameterCode = "SURFACE",
                MeasuredValue = "OK",
                Result = QualityMeasurementResult.Pass,
                Status = EntityStatus.Active,
            },
            new QualityMeasurement
            {
                TenantId = tenant.Id,
                QualityCheckId = qcFinal.Id,
                ParameterCode = "GLOSS",
                MeasuredValue = "82",
                TargetValue = "80",
                MinValue = "75",
                MaxValue = "90",
                Result = QualityMeasurementResult.Pass,
                Status = EntityStatus.Active,
            },
            new QualityMeasurement
            {
                TenantId = tenant.Id,
                QualityCheckId = qcFinal.Id,
                ParameterCode = "COLOR_DELTA",
                MeasuredValue = "0.6",
                TargetValue = "1.0",
                Result = QualityMeasurementResult.NotEvaluated,
                Status = EntityStatus.Active,
            },
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
            MachineId = machines[2].Id,
            AlarmCode = "PROC-TEMP-HIGH",
            AlarmText = "Demo: process temperature high (placeholder).",
            Severity = AlarmSeverity.Critical,
            Category = "process",
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-40),
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
            MachineId = machines[2].Id,
            ProductionOrderId = order2.Id,
            DowntimeReasonCode = "WAIT-MAT",
            DowntimeReasonText = "Demo material wait",
            PlannedFlag = false,
            StartTime = DateTimeOffset.UtcNow.AddHours(-2),
            DurationMs = null,
            EnteredBy = userOperator.Id,
            Status = EntityStatus.Active,
        };

        var oeeMachine = new OeeSnapshot
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line1.Id,
            MachineId = machines[0].Id,
            PeriodType = OeePeriodType.Shift,
            PeriodStart = DateTimeOffset.UtcNow.AddHours(-8),
            PeriodEnd = DateTimeOffset.UtcNow,
            Availability = 0.92m,
            Performance = 0.88m,
            Quality = 0.99m,
            OeeValue = 0.80m,
            Status = EntityStatus.Active,
        };

        var oeeLine = new OeeSnapshot
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            LineId = line1.Id,
            PeriodType = OeePeriodType.Day,
            PeriodStart = UtcStartOfToday(),
            PeriodEnd = UtcStartOfToday().AddDays(1),
            Availability = 0.90m,
            Performance = 0.85m,
            Quality = 0.98m,
            OeeValue = 0.75m,
            Status = EntityStatus.Active,
        };

        var oeeSite = new OeeSnapshot
        {
            TenantId = tenant.Id,
            SiteId = site.Id,
            PeriodType = OeePeriodType.Day,
            PeriodStart = UtcStartOfToday(),
            PeriodEnd = UtcStartOfToday().AddDays(1),
            Availability = 0.88m,
            Performance = 0.82m,
            Quality = 0.97m,
            OeeValue = 0.70m,
            Status = EntityStatus.Active,
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

        var wfDefRecipe = new WorkflowDefinition
        {
            TenantId = tenant.Id,
            WorkflowType = "recipe_approval",
            Name = "Recipe publish approval",
            Status = EntityStatus.Active,
        };

        var wfDefQuality = new WorkflowDefinition
        {
            TenantId = tenant.Id,
            WorkflowType = "quality_approval",
            Name = "Quality check / disposition approval",
            Status = EntityStatus.Active,
        };

        var wfDefOrderClose = new WorkflowDefinition
        {
            TenantId = tenant.Id,
            WorkflowType = "order_close_approval",
            Name = "Production order close approval",
            Status = EntityStatus.Active,
        };

        var wfDefMaint = new WorkflowDefinition
        {
            TenantId = tenant.Id,
            WorkflowType = "maintenance_close_approval",
            Name = "Maintenance work order close (placeholder type)",
            Status = EntityStatus.Active,
        };

        var wfRecipeStep1 = new WorkflowStep
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefRecipe.Id,
            SequenceNo = 10,
            RoleCode = ApplicationRoles.Quality,
            ApprovalMode = WorkflowApprovalMode.Sequential,
            Status = EntityStatus.Active,
        };

        var wfRecipeStep2 = new WorkflowStep
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefRecipe.Id,
            SequenceNo = 20,
            RoleCode = ApplicationRoles.TenantAdmin,
            ApprovalMode = WorkflowApprovalMode.Sequential,
            Status = EntityStatus.Active,
        };

        var wfQualityStep1 = new WorkflowStep
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefQuality.Id,
            SequenceNo = 10,
            RoleCode = ApplicationRoles.TenantAdmin,
            ApprovalMode = WorkflowApprovalMode.ParallelAny,
            Status = EntityStatus.Active,
        };

        var wfQualityStep2 = new WorkflowStep
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefQuality.Id,
            SequenceNo = 20,
            RoleCode = ApplicationRoles.Quality,
            ApprovalMode = WorkflowApprovalMode.Sequential,
            Status = EntityStatus.Active,
        };

        var wfOrderStep1 = new WorkflowStep
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefOrderClose.Id,
            SequenceNo = 10,
            RoleCode = ApplicationRoles.ProductionManager,
            ApprovalMode = WorkflowApprovalMode.Sequential,
            Status = EntityStatus.Active,
        };

        var wfOrderStep2 = new WorkflowStep
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefOrderClose.Id,
            SequenceNo = 20,
            RoleCode = ApplicationRoles.TenantAdmin,
            ApprovalMode = WorkflowApprovalMode.Sequential,
            Status = EntityStatus.Active,
        };

        var wfMaintStep1 = new WorkflowStep
        {
            TenantId = tenant.Id,
            WorkflowDefinitionId = wfDefMaint.Id,
            SequenceNo = 10,
            RoleCode = ApplicationRoles.Maintenance,
            ApprovalMode = WorkflowApprovalMode.Sequential,
            Status = EntityStatus.Active,
        };

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
        db.AddRange(userOperator, userSupervisor, userDevAdmin);
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
        db.AddRange(order1, order2);
        db.AddRange(op1a, op1b, op2a, op2b);
        db.AddRange(assign1, assign2);
        db.AddRange(lot1, lot2);
        db.AddRange(matCons);
        db.AddRange(prodDecl);
        db.Add(scrap1);
        db.AddRange(invMoves);
        db.AddRange(shiftMorning, shiftAfternoon);
        db.AddRange(empAssign);
        db.AddRange(qcLine, qcFinal);
        db.AddRange(qualityMeasurements);
        db.Add(ncDemo);
        db.Add(dispRework);
        db.AddRange(alarmDrive, alarmSite, alarmLine2);
        db.AddRange(dtChangeover, dtBreakdown, dtLineStop);
        db.AddRange(oeeMachine, oeeLine, oeeSite);
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

        machines[0].OperationalStatus = MachineOperationalStatus.Down;
        machines[1].OperationalStatus = MachineOperationalStatus.Running;
        machines[2].OperationalStatus = MachineOperationalStatus.PmDue;
        machines[3].OperationalStatus = MachineOperationalStatus.Running;

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
            MachineId = machines[2].Id,
            Name = "Quarterly mold inspection",
            IntervalDays = 90,
            LastDoneAt = DateTimeOffset.UtcNow.AddDays(-92),
            NextDueAt = DateTimeOffset.UtcNow.AddDays(-2),
            OwnerRole = "mold",
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
            MachineId = machines[2].Id,
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

        db.AddRange(pmM101, pmM102, pmM201);
        db.AddRange(wo1, wo2, wo3);
        db.AddRange(fault1, fault2);

        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Dev seed completed.");
    }

    // UTC midnight (offset 0); DateTimeOffset.UtcNow.Date would be Unspecified→local offset and breaks Npgsql timestamptz.
    private static DateTimeOffset UtcStartOfToday() =>
        new DateTimeOffset(DateTimeOffset.UtcNow.UtcDateTime.Date, TimeSpan.Zero);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

