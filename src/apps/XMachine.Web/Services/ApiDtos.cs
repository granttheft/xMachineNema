namespace XMachine.Web.Services;

public sealed record MesSummaryDto(int Orders, int Recipes, int Lots, int Shifts);

public sealed record QualitySummaryDto(int Checks, int Measurements, int Nonconformances, int Dispositions);

public sealed record EventingSummaryDto(int Alarms, int Downtimes, int Oee, int KpiDefs, int KpiResults);

public sealed record IntegrationOperationalDto(int Definitions, int Instances, int Profiles);

public sealed record IntegrationRuntimeDto(IReadOnlyList<string>? RegistryCodes, int ActiveHealthRows);

public sealed record IntegrationHealthSummaryDto(IntegrationOperationalDto Operational, IntegrationRuntimeDto Runtime);

public sealed record ProductionOrderRowDto(Guid Id, string OrderNo, string ProductCode, string OrderStatus, decimal QuantityPlanned, decimal QuantityCompleted, Guid? LineId, Guid? SiteId, string Status);

public sealed record RecipeRowDto(Guid Id, string Code, string Name, int Version, string Status);

public sealed record LotRowDto(Guid Id, string LotNo, Guid ProductionOrderId, string LotStatus, decimal QuantityGood, Guid? LineId, Guid? MachineId, string Status);

public sealed record ShiftRowDto(Guid Id, string Code, string Name, DateOnly ShiftDate, Guid SiteId, Guid? LineId, string Lifecycle, string Status);

public sealed record QualityCheckRowDto(Guid Id, string CheckType, DateTimeOffset CheckTime, string CheckStatus, Guid? ProductionOrderId, Guid? LotBatchId, Guid? MachineId, Guid? ApprovedBy, string Status);

public sealed record NonconformanceRowDto(Guid Id, Guid QualityCheckId, string NcCode, string Description, string Severity, string NcStatus, string Status);

public sealed record AlarmRowDto(Guid Id, string AlarmCode, string AlarmText, string Severity, string Category, DateTimeOffset StartTime, DateTimeOffset? EndTime, long? DurationMs, Guid? SiteId, Guid? LineId, Guid? MachineId, string AlarmStatus, string Status);

public sealed record DowntimeRowDto(Guid Id, string DowntimeReasonCode, string? DowntimeReasonText, bool PlannedFlag, DateTimeOffset StartTime, DateTimeOffset? EndTime, long? DurationMs, Guid? MachineId, Guid? LineId, Guid? ProductionOrderId, Guid? EnteredBy, string Status);

public sealed record OeeRowDto(Guid Id, string PeriodType, DateTimeOffset PeriodStart, DateTimeOffset PeriodEnd, Guid? SiteId, Guid? LineId, Guid? MachineId, decimal Availability, decimal Performance, decimal Quality, decimal OeeValue, string Status);

public sealed record KpiDefinitionRowDto(Guid Id, string Code, string Name, string ScopeType, string DataSourceType, string Status);

public sealed record KpiResultRowDto(Guid Id, Guid KpiDefinitionId, string ScopeType, Guid ScopeId, DateTimeOffset PeriodStart, DateTimeOffset PeriodEnd, decimal Value, string Status);

public sealed record KpisBundleDto(IReadOnlyList<KpiDefinitionRowDto> Definitions, IReadOnlyList<KpiResultRowDto> Results);

public sealed record ConnectorDefinitionRowDto(Guid Id, string Code, string Name, string Category, string Direction, string Status);

public sealed record ConnectorInstanceRowDto(Guid Id, string Code, string Name, Guid ConnectorDefinitionId, Guid? SiteId, string Status);

public sealed record MappingProfileRowDto(Guid Id, string Name, int Version, Guid ConnectorInstanceId, string Status);

public sealed record PlatformSummaryDto(int Tenants, int Enterprises, int Sites, int Lines, int Machines, int Buildings, int Stations);

public sealed record PlatformTenantRowDto(Guid Id, string Code, string Name, string Status, int SiteCount, int LineCount, int MachineCount);

public sealed record PlatformSiteRowDto(Guid Id, Guid TenantId, Guid EnterpriseId, string Code, string Name, string Status);

public sealed record PlatformLineRowDto(Guid Id, Guid TenantId, Guid SiteId, Guid? BuildingId, string Code, string Name, string Status);

public sealed record PlatformMachineRowDto(Guid Id, Guid TenantId, Guid LineId, string Code, string Name, string Status);

public sealed record CommercialSummaryDto(int Licenses, int Modules, int ModuleActivations, int LicensedLines);

public sealed record LicenseRowDto(Guid Id, Guid TenantId, string LicenseType, DateTimeOffset? ValidFrom, DateTimeOffset? ValidTo, string Status);

public sealed record CommercialModuleRowDto(Guid Id, string Code, string Name, string Status);

public sealed record ModuleActivationRowDto(Guid Id, Guid TenantId, Guid ModuleId, string ModuleCode, string ModuleName, DateTimeOffset ActivatedAt, string Status);

public sealed record LicensedLineRowDto(Guid Id, Guid TenantId, Guid LineId, string LineCode, string LineName, string Status);

public sealed record WorkflowSummaryDto(int Definitions, int Steps, int Instances, int Actions);

public sealed record WorkflowDefinitionRowDto(Guid Id, Guid TenantId, string WorkflowType, string Name, string Status, int StepCount);

public sealed record WorkflowInstanceRowDto(
    Guid Id,
    Guid TenantId,
    Guid WorkflowDefinitionId,
    string ReferenceType,
    Guid ReferenceId,
    string WorkflowState,
    DateTimeOffset? StartedAt,
    DateTimeOffset? EndedAt,
    string Status);
