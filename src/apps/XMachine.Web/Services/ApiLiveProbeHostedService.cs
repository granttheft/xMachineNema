namespace XMachine.Web.Services;



/// <summary>

/// After host start, probes <c>GET {api}/health/live</c> once so the shell can show a simple banner if the API is down.

/// </summary>

internal sealed class ApiLiveProbeHostedService : IHostedService

{

    private readonly IHostApplicationLifetime _lifetime;

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly IConfiguration _configuration;

    private readonly ApiConnectivityState _state;

    private readonly ILogger<ApiLiveProbeHostedService> _logger;



    public ApiLiveProbeHostedService(

        IHostApplicationLifetime lifetime,

        IHttpClientFactory httpClientFactory,

        IConfiguration configuration,

        ApiConnectivityState state,

        ILogger<ApiLiveProbeHostedService> logger)

    {

        _lifetime = lifetime;

        _httpClientFactory = httpClientFactory;

        _configuration = configuration;

        _state = state;

        _logger = logger;

    }



    public Task StartAsync(CancellationToken cancellationToken)

    {

        _lifetime.ApplicationStarted.Register(() => _ = RunProbeAsync());

        return Task.CompletedTask;

    }



    private async Task RunProbeAsync()

    {

        try

        {

            await Task.Delay(750, _lifetime.ApplicationStopping).ConfigureAwait(false);

        }

        catch (OperationCanceledException)

        {

            return;

        }



        var baseUrl = ApiBaseUrlResolver.Resolve(_configuration);

        var liveUrl = baseUrl.TrimEnd('/') + "/health/live";



        try

        {

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.ApplicationStopping);

            cts.CancelAfter(TimeSpan.FromSeconds(6));

            var client = _httpClientFactory.CreateClient(nameof(ApiLiveProbeHostedService));

            var response = await client.GetAsync(liveUrl, cts.Token).ConfigureAwait(false);

            var ok = response.IsSuccessStatusCode;

            _state.SetResult(ok, baseUrl);

            if (!ok)

            {

                _logger.LogWarning(

                    "API live health check returned {Status}. Expected 2xx from GET {LiveUrl}.",

                    (int)response.StatusCode,

                    liveUrl);

            }

        }

        catch (Exception ex)

        {

            _state.SetResult(false, baseUrl);

            _logger.LogWarning(ex, "API live health check failed for GET {LiveUrl}.", liveUrl);

        }

    }



    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}


