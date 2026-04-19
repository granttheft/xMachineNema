using System.Net.Http.Json;

namespace XMachine.Web.Services;

public sealed class XMachineApiClient(HttpClient http)
{
    private async Task<ApiFetch<T>> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.GetAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var detail = string.IsNullOrWhiteSpace(body) ? response.ReasonPhrase : body;
                return ApiFetch<T>.Fail($"{(int)response.StatusCode}: {detail}");
            }

            var data = await response.Content.ReadFromJsonAsync<T>(ApiJson.Options, cancellationToken).ConfigureAwait(false);
            if (data is null)
                return ApiFetch<T>.Fail("Empty response body.");
            return ApiFetch<T>.Ok(data);
        }
        catch (Exception ex)
        {
            return ApiFetch<T>.Fail(ex.Message);
        }
    }

    public Task<ApiFetch<MesSummaryDto>> GetMesSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<MesSummaryDto>("api/mes/summary", cancellationToken);

    public Task<ApiFetch<List<ProductionOrderRowDto>>> GetProductionOrdersAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ProductionOrderRowDto>>("api/mes/production-orders", cancellationToken);

    public Task<ApiFetch<List<RecipeRowDto>>> GetRecipesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<RecipeRowDto>>("api/mes/recipes", cancellationToken);

    public Task<ApiFetch<List<LotRowDto>>> GetLotsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<LotRowDto>>("api/mes/lots", cancellationToken);

    public Task<ApiFetch<List<ShiftRowDto>>> GetShiftsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ShiftRowDto>>("api/mes/shifts", cancellationToken);

    public Task<ApiFetch<QualitySummaryDto>> GetQualitySummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<QualitySummaryDto>("api/quality/summary", cancellationToken);

    public Task<ApiFetch<List<QualityCheckRowDto>>> GetQualityChecksAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<QualityCheckRowDto>>("api/quality/checks", cancellationToken);

    public Task<ApiFetch<List<NonconformanceRowDto>>> GetNonconformancesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<NonconformanceRowDto>>("api/quality/nonconformances", cancellationToken);

    public Task<ApiFetch<EventingSummaryDto>> GetEventingSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<EventingSummaryDto>("api/eventing/summary", cancellationToken);

    public Task<ApiFetch<List<AlarmRowDto>>> GetAlarmsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<AlarmRowDto>>("api/eventing/alarms", cancellationToken);

    public Task<ApiFetch<List<DowntimeRowDto>>> GetDowntimesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<DowntimeRowDto>>("api/eventing/downtimes", cancellationToken);

    public Task<ApiFetch<List<OeeRowDto>>> GetOeeSnapshotsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<OeeRowDto>>("api/eventing/oee", cancellationToken);

    public Task<ApiFetch<KpisBundleDto>> GetKpisAsync(CancellationToken cancellationToken = default) =>
        GetAsync<KpisBundleDto>("api/eventing/kpis", cancellationToken);

    public Task<ApiFetch<List<ConnectorDefinitionRowDto>>> GetConnectorDefinitionsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ConnectorDefinitionRowDto>>("api/integration/connector-definitions", cancellationToken);

    public Task<ApiFetch<List<ConnectorInstanceRowDto>>> GetConnectorInstancesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ConnectorInstanceRowDto>>("api/integration/connector-instances", cancellationToken);

    public Task<ApiFetch<List<MappingProfileRowDto>>> GetMappingProfilesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<MappingProfileRowDto>>("api/integration/mapping-profiles", cancellationToken);

    public Task<ApiFetch<IntegrationHealthSummaryDto>> GetIntegrationHealthAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IntegrationHealthSummaryDto>("api/integration/health/summary", cancellationToken);

    public Task<ApiFetch<PlatformSummaryDto>> GetPlatformSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<PlatformSummaryDto>("api/platform/summary", cancellationToken);

    public Task<ApiFetch<List<PlatformTenantRowDto>>> GetPlatformTenantsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<PlatformTenantRowDto>>("api/platform/tenants", cancellationToken);

    public Task<ApiFetch<List<PlatformSiteRowDto>>> GetPlatformSitesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<PlatformSiteRowDto>>("api/platform/sites", cancellationToken);

    public Task<ApiFetch<List<PlatformLineRowDto>>> GetPlatformLinesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<PlatformLineRowDto>>("api/platform/lines", cancellationToken);

    public Task<ApiFetch<List<PlatformMachineRowDto>>> GetPlatformMachinesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<PlatformMachineRowDto>>("api/platform/machines", cancellationToken);

    public Task<ApiFetch<CommercialSummaryDto>> GetCommercialSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<CommercialSummaryDto>("api/commercial/summary", cancellationToken);

    public Task<ApiFetch<List<LicenseRowDto>>> GetCommercialLicensesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<LicenseRowDto>>("api/commercial/licenses", cancellationToken);

    public Task<ApiFetch<List<CommercialModuleRowDto>>> GetCommercialModulesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<CommercialModuleRowDto>>("api/commercial/modules", cancellationToken);

    public Task<ApiFetch<List<ModuleActivationRowDto>>> GetCommercialModuleActivationsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ModuleActivationRowDto>>("api/commercial/module-activations", cancellationToken);

    public Task<ApiFetch<List<LicensedLineRowDto>>> GetCommercialLicensedLinesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<LicensedLineRowDto>>("api/commercial/licensed-lines", cancellationToken);

    public Task<ApiFetch<WorkflowSummaryDto>> GetWorkflowSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<WorkflowSummaryDto>("api/workflow/summary", cancellationToken);

    public Task<ApiFetch<List<WorkflowDefinitionRowDto>>> GetWorkflowDefinitionsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<WorkflowDefinitionRowDto>>("api/workflow/definitions", cancellationToken);

    public Task<ApiFetch<List<WorkflowInstanceRowDto>>> GetWorkflowInstancesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<WorkflowInstanceRowDto>>("api/workflow/instances", cancellationToken);
}
