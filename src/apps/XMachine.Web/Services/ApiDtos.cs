namespace XMachine.Web.Services;

public sealed record MesSummaryDto(int Orders, int Recipes, int Lots, int Shifts);

public sealed record QualitySummaryDto(int Checks, int Measurements, int Nonconformances, int Dispositions);

public sealed record EventingSummaryDto(int Alarms, int Downtimes, int Oee, int KpiDefs, int KpiResults);

public sealed record IntegrationOperationalDto(int Definitions, int Instances, int Profiles);

public sealed record IntegrationRuntimeDto(IReadOnlyList<string>? RegistryCodes, int ActiveHealthRows);

public sealed record IntegrationHealthSummaryDto(IntegrationOperationalDto Operational, IntegrationRuntimeDto Runtime);

public sealed record ProductionOrderRowDto(Guid Id, string OrderNo, string ProductCode, int OrderStatus, decimal QuantityPlanned, decimal QuantityCompleted, Guid? LineId, Guid? SiteId, int Status);

public sealed record RecipeRowDto(Guid Id, string Code, string Name, int Version, int Status);

public sealed record LotRowDto(Guid Id, string LotNo, Guid ProductionOrderId, int LotStatus, decimal QuantityGood, Guid? LineId, Guid? MachineId, int Status);

public sealed record ShiftRowDto(Guid Id, string Code, string Name, DateOnly ShiftDate, Guid SiteId, Guid? LineId, int Lifecycle, int Status);

public sealed record QualityCheckRowDto(Guid Id, string CheckType, DateTimeOffset CheckTime, int CheckStatus, Guid? ProductionOrderId, Guid? LotBatchId, Guid? MachineId, Guid? ApprovedBy, int Status);

public sealed record NonconformanceRowDto(Guid Id, Guid QualityCheckId, string NcCode, string Description, int Severity, int NcStatus, int Status);

public sealed record AlarmRowDto(Guid Id, string AlarmCode, string AlarmText, int Severity, string Category, DateTimeOffset StartTime, DateTimeOffset? EndTime, long? DurationMs, Guid? SiteId, Guid? LineId, Guid? MachineId, int AlarmStatus, int Status);

public sealed record DowntimeRowDto(Guid Id, string DowntimeReasonCode, string? DowntimeReasonText, bool PlannedFlag, DateTimeOffset StartTime, DateTimeOffset? EndTime, long? DurationMs, Guid? MachineId, Guid? LineId, Guid? ProductionOrderId, Guid? EnteredBy, int Status);

public sealed record OeeRowDto(Guid Id, int PeriodType, DateTimeOffset PeriodStart, DateTimeOffset PeriodEnd, Guid? SiteId, Guid? LineId, Guid? MachineId, decimal Availability, decimal Performance, decimal Quality, decimal OeeValue, int Status);

public sealed record KpiDefinitionRowDto(Guid Id, string Code, string Name, string ScopeType, string DataSourceType, int Status);

public sealed record KpiResultRowDto(Guid Id, Guid KpiDefinitionId, string ScopeType, Guid ScopeId, DateTimeOffset PeriodStart, DateTimeOffset PeriodEnd, decimal Value, int Status);

public sealed record KpisBundleDto(IReadOnlyList<KpiDefinitionRowDto> Definitions, IReadOnlyList<KpiResultRowDto> Results);

public sealed record ConnectorDefinitionRowDto(Guid Id, string Code, string Name, string Category, int Direction, int Status);

public sealed record ConnectorInstanceRowDto(Guid Id, string Code, string Name, Guid ConnectorDefinitionId, Guid? SiteId, int Status);

public sealed record MappingProfileRowDto(Guid Id, string Name, int Version, Guid ConnectorInstanceId, int Status);
