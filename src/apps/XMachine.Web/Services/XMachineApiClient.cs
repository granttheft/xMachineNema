using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace XMachine.Web.Services;

public sealed class XMachineApiClient(HttpClient http)
{
    private const int MaxErrorBodyChars = 600;

    private async Task<ApiFetch<T>> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.GetAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var detail = string.IsNullOrWhiteSpace(body)
                    ? response.ReasonPhrase ?? ""
                    : TrimErrorBody(body);
                var code = (int)response.StatusCode;
                var message = string.IsNullOrWhiteSpace(detail)
                    ? $"{code} {response.StatusCode}"
                    : $"{code}: {detail}";
                message += StatusCodeHint(code);
                return ApiFetch<T>.Fail(message);
            }

            var data = await response.Content.ReadFromJsonAsync<T>(ApiJson.Options, cancellationToken).ConfigureAwait(false);
            if (data is null)
                return ApiFetch<T>.Fail("Empty response body.");
            return ApiFetch<T>.Ok(data);
        }
        catch (JsonException jex)
        {
            return ApiFetch<T>.Fail($"Could not parse API JSON ({relativeUrl}): {jex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return ApiFetch<T>.Fail(
                $"Could not reach the API at {http.BaseAddress?.AbsoluteUri.TrimEnd('/') ?? "?"}. {ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiFetch<T>.Fail(ex.Message);
        }
    }

    private async Task<ApiFetch<TResponse>> ParseJsonResponseAsync<TResponse>(
        HttpResponseMessage response,
        string relativeUrl,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var detail = string.IsNullOrWhiteSpace(body)
                ? response.ReasonPhrase ?? ""
                : TrimErrorBody(body);
            var code = (int)response.StatusCode;
            var message = string.IsNullOrWhiteSpace(detail)
                ? $"{code} {response.StatusCode}"
                : $"{code}: {detail}";
            message += StatusCodeHint(code);
            return ApiFetch<TResponse>.Fail(message);
        }

        try
        {
            var data = await response.Content.ReadFromJsonAsync<TResponse>(ApiJson.Options, cancellationToken).ConfigureAwait(false);
            if (data is null)
                return ApiFetch<TResponse>.Fail("Empty response body.");
            return ApiFetch<TResponse>.Ok(data);
        }
        catch (JsonException jex)
        {
            return ApiFetch<TResponse>.Fail($"Could not parse API JSON ({relativeUrl}): {jex.Message}");
        }
    }

    private async Task<ApiFetch<TResponse>> PostJsonAsync<TRequest, TResponse>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken)
    {
        try
        {
            using var content = JsonContent.Create(body, options: ApiJson.Options);
            var response = await http.PostAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
            return await ParseJsonResponseAsync<TResponse>(response, relativeUrl, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            return ApiFetch<TResponse>.Fail(
                $"Could not reach the API at {http.BaseAddress?.AbsoluteUri.TrimEnd('/') ?? "?"}. {ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiFetch<TResponse>.Fail(ex.Message);
        }
    }

    private async Task<ApiFetch<TResponse>> PutJsonAsync<TRequest, TResponse>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken)
    {
        try
        {
            using var content = JsonContent.Create(body, options: ApiJson.Options);
            var response = await http.PutAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
            return await ParseJsonResponseAsync<TResponse>(response, relativeUrl, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            return ApiFetch<TResponse>.Fail(
                $"Could not reach the API at {http.BaseAddress?.AbsoluteUri.TrimEnd('/') ?? "?"}. {ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiFetch<TResponse>.Fail(ex.Message);
        }
    }

    private async Task<ApiFetch<bool>> DeleteAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.DeleteAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var detail = string.IsNullOrWhiteSpace(body)
                    ? response.ReasonPhrase ?? ""
                    : TrimErrorBody(body);
                var code = (int)response.StatusCode;
                var message = string.IsNullOrWhiteSpace(detail)
                    ? $"{code} {response.StatusCode}"
                    : $"{code}: {detail}";
                message += StatusCodeHint(code);
                return ApiFetch<bool>.Fail(message);
            }

            return ApiFetch<bool>.Ok(true);
        }
        catch (HttpRequestException ex)
        {
            return ApiFetch<bool>.Fail(
                $"Could not reach the API at {http.BaseAddress?.AbsoluteUri.TrimEnd('/') ?? "?"}. {ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiFetch<bool>.Fail(ex.Message);
        }
    }

    private static string TrimErrorBody(string body)
    {
        var t = body.Trim();
        if (t.Length <= MaxErrorBodyChars)
            return t;
        return t[..MaxErrorBodyChars] + "…";
    }

    private static string StatusCodeHint(int statusCode) => statusCode switch
    {
        (int)HttpStatusCode.Unauthorized =>
            " Sign in on the Web app (session cookie must reach the API). Platform and commercial list endpoints require authentication.",
        (int)HttpStatusCode.Forbidden =>
            " Your account may lack the TenantAdmin or SuperAdmin role required for this API route.",
        _ => "",
    };

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

    /// <summary>Creates an ongoing downtime record (POST /api/eventing/downtimes).</summary>
    public async Task<(bool Success, string? Error)> PostDowntimeAsync(
        CreateDowntimeDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await PostJsonAsync<CreateDowntimeDto, CreateDowntimeResponseDto>(
            "api/eventing/downtimes",
            dto,
            cancellationToken).ConfigureAwait(false);
        return result.Error is null ? (true, null) : (false, result.Error);
    }

    /// <summary>Acknowledges an alarm (PUT /api/eventing/alarms/{alarmId}/ack).</summary>
    public async Task<(bool Success, string? Error)> AcknowledgeAlarmAsync(
        Guid alarmId,
        AcknowledgeAlarmDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await PutJsonAsync<AcknowledgeAlarmDto, AcknowledgeAlarmResponseDto>(
            $"api/eventing/alarms/{alarmId}/ack",
            dto,
            cancellationToken).ConfigureAwait(false);
        return result.Error is null ? (true, null) : (false, result.Error);
    }

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

    public Task<ApiFetch<List<ApiMachine>>> GetEngineeringMachineStatusAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiMachine>>("api/engineering/machine-status", cancellationToken);

    public Task<ApiFetch<List<ApiWorkOrder>>> GetEngineeringWorkOrdersAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiWorkOrder>>("api/engineering/work-orders", cancellationToken);

    public Task<ApiFetch<List<ApiPmSchedule>>> GetEngineeringPmSchedulesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiPmSchedule>>("api/engineering/pm-schedules", cancellationToken);

    public Task<ApiFetch<ApiSummary>> GetEngineeringSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<ApiSummary>("api/engineering/summary", cancellationToken);

    public Task<ApiFetch<List<ApiJobExecution>>> GetProductionJobsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiJobExecution>>("api/production/jobs", cancellationToken);

    public Task<ApiFetch<List<ApiJobExecution>>> GetProductionActiveJobsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiJobExecution>>("api/production/jobs/active", cancellationToken);

    public Task<ApiFetch<List<ApiMachineWithJob>>> GetProductionMachinesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiMachineWithJob>>("api/production/machines", cancellationToken);

    public Task<ApiFetch<ApiProductionSummary>> GetProductionSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<ApiProductionSummary>("api/production/summary", cancellationToken);

    /// <summary>Lists production plans (GET /api/planning/plans).</summary>
    public Task<ApiFetch<List<ApiProductionPlan>>> GetPlanningPlansAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiProductionPlan>>("api/planning/plans", cancellationToken);

    /// <summary>Lists active production plans (GET /api/planning/plans/active).</summary>
    public Task<ApiFetch<List<ApiProductionPlan>>> GetPlanningActivePlansAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiProductionPlan>>("api/planning/plans/active", cancellationToken);

    /// <summary>Gets a plan with slots (GET /api/planning/plans/{id}).</summary>
    public Task<ApiFetch<ApiPlanWithSlots>> GetPlanningPlanAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<ApiPlanWithSlots>($"api/planning/plans/{id}", cancellationToken);

    /// <summary>Lists all plan slots (GET /api/planning/slots).</summary>
    public Task<ApiFetch<List<ApiPlanSlot>>> GetPlanningSlotsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiPlanSlot>>("api/planning/slots", cancellationToken);

    /// <summary>Lists plan slots for a machine (GET /api/planning/slots/by-machine/{machineId}).</summary>
    public Task<ApiFetch<List<ApiPlanSlotWithPlan>>> GetPlanningSlotsByMachineAsync(
        Guid machineId,
        CancellationToken cancellationToken = default) =>
        GetAsync<List<ApiPlanSlotWithPlan>>($"api/planning/slots/by-machine/{machineId}", cancellationToken);

    /// <summary>Planning KPI summary (GET /api/planning/summary).</summary>
    public Task<ApiFetch<ApiPlanningSummary>> GetPlanningSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<ApiPlanningSummary>("api/planning/summary", cancellationToken);

    public Task<ApiFetch<CreateWorkOrderResponse>> PostWorkOrderAsync(CreateWorkOrderDto dto, CancellationToken cancellationToken = default) =>
        PostJsonAsync<CreateWorkOrderDto, CreateWorkOrderResponse>("api/engineering/work-orders", dto, cancellationToken);

    public Task<ApiFetch<CreatePmScheduleResponse>> PostPmScheduleAsync(CreatePmScheduleDto dto, CancellationToken cancellationToken = default) =>
        PostJsonAsync<CreatePmScheduleDto, CreatePmScheduleResponse>("api/engineering/pm-schedules", dto, cancellationToken);

    public Task<ApiFetch<UpdateWorkOrderStatusResponse>> PutWorkOrderStatusAsync(
        Guid id,
        UpdateWorkOrderStatusDto dto,
        CancellationToken cancellationToken = default) =>
        PutJsonAsync<UpdateWorkOrderStatusDto, UpdateWorkOrderStatusResponse>(
            $"api/engineering/work-orders/{id}/status",
            dto,
            cancellationToken);

    public Task<ApiFetch<UpdateMachineStatusResponse>> PutMachineStatusAsync(
        Guid id,
        UpdateMachineStatusDto dto,
        CancellationToken cancellationToken = default) =>
        PutJsonAsync<UpdateMachineStatusDto, UpdateMachineStatusResponse>(
            $"api/engineering/machine-status/{id}",
            dto,
            cancellationToken);

    public Task<ApiFetch<CreateFaultResponse>> PostFaultAsync(CreateFaultDto dto, CancellationToken cancellationToken = default) =>
        PostJsonAsync<CreateFaultDto, CreateFaultResponse>("api/engineering/faults", dto, cancellationToken);

    /// <summary>Creates a queued job execution (POST /api/production/jobs).</summary>
    public Task<ApiFetch<CreateProductionJobResponse>> PostProductionJobAsync(
        CreateJobDto dto,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<CreateJobDto, CreateProductionJobResponse>("api/production/jobs", dto, cancellationToken);

    /// <summary>Updates job execution status (PUT /api/production/jobs/{id}/status).</summary>
    public Task<ApiFetch<UpdateJobStatusResponse>> PutJobStatusAsync(
        Guid id,
        UpdateJobStatusDto dto,
        CancellationToken cancellationToken = default) =>
        PutJsonAsync<UpdateJobStatusDto, UpdateJobStatusResponse>(
            $"api/production/jobs/{id}/status",
            dto,
            cancellationToken);

    /// <summary>Posts an operator production declaration (POST /api/production/declarations).</summary>
    public Task<ApiFetch<CreateDeclarationResponse>> PostDeclarationAsync(
        CreateDeclarationDto dto,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<CreateDeclarationDto, CreateDeclarationResponse>("api/production/declarations", dto, cancellationToken);

    /// <summary>Assigns or unassigns a job/operator on a machine (PUT /api/production/machines/{id}/job).</summary>
    public Task<ApiFetch<UpdateMachineJobResponse>> PutMachineJobAsync(
        Guid id,
        UpdateMachineJobDto dto,
        CancellationToken cancellationToken = default) =>
        PutJsonAsync<UpdateMachineJobDto, UpdateMachineJobResponse>(
            $"api/production/machines/{id}/job",
            dto,
            cancellationToken);

    /// <summary>Lists supported languages and active override counts (GET /api/admin/languages).</summary>
    public Task<ApiFetch<List<LanguageSummaryRowDto>>> GetAdminLanguagesAsync(
        CancellationToken cancellationToken = default) =>
        GetAsync<List<LanguageSummaryRowDto>>("api/admin/languages", cancellationToken);

    /// <summary>Lists active translation overrides for a language (GET /api/admin/languages/{code}/overrides).</summary>
    public Task<ApiFetch<List<TranslationOverrideRowDto>>> GetTranslationOverridesAsync(
        string languageCode,
        CancellationToken cancellationToken = default) =>
        GetAsync<List<TranslationOverrideRowDto>>(
            $"api/admin/languages/{Uri.EscapeDataString(languageCode)}/overrides",
            cancellationToken);

    /// <summary>Creates or updates a translation override (POST /api/admin/languages/{code}/overrides).</summary>
    public Task<ApiFetch<UpsertTranslationOverrideResponseDto>> UpsertTranslationOverrideAsync(
        string languageCode,
        UpsertTranslationOverrideRequestDto body,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<UpsertTranslationOverrideRequestDto, UpsertTranslationOverrideResponseDto>(
            $"api/admin/languages/{Uri.EscapeDataString(languageCode)}/overrides",
            body,
            cancellationToken);

    /// <summary>Soft-deletes a translation override (DELETE /api/admin/languages/{code}/overrides/{key}).</summary>
    public Task<ApiFetch<bool>> DeleteTranslationOverrideAsync(
        string languageCode,
        string key,
        CancellationToken cancellationToken = default) =>
        DeleteAsync(
            $"api/admin/languages/{Uri.EscapeDataString(languageCode)}/overrides/{Uri.EscapeDataString(key)}",
            cancellationToken);
}
