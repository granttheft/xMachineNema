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
}
