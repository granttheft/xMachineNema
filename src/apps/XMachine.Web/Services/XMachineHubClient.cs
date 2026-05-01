using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Claims;
using XMachine.Module.Auth.Security;
using XMachine.Web.Auth;

namespace XMachine.Web.Services;

/// <summary>
/// Singleton SignalR client for XMachine.Api monitoring hub; forwards auth cookie like REST calls.
/// </summary>
public sealed class XMachineHubClient : IAsyncDisposable
{
    private const string HubUrlConfigKey = "XMachine:HubUrl";

    /// <summary>
    /// Reads <c>XMachine:HubUrl</c>; when empty, derives <c>{ApiBase}/hubs/xmachine</c>.
    /// </summary>
    public static string ResolveConfiguredHubUrl(IConfiguration configuration)
    {
        var hub = configuration[HubUrlConfigKey];
        if (!string.IsNullOrWhiteSpace(hub))
            return hub.Trim();

        var api = ApiBaseUrlResolver.Resolve(configuration).TrimEnd('/');
        return $"{api}/hubs/xmachine";
    }

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserCookieStore _cookieStore;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<XMachineHubClient> _logger;

    private readonly SemaphoreSlim _startGate = new(1, 1);
    private HubConnection? _connection;
    private string? _negotiatedCookieHeader;

    /// <summary>Initializes the hub client.</summary>
    public XMachineHubClient(
        IHttpContextAccessor httpContextAccessor,
        UserCookieStore cookieStore,
        IServiceScopeFactory scopeFactory,
        ILogger<XMachineHubClient> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _cookieStore = cookieStore;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>True when the hub connection is in the Connected state.</summary>
    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    /// <summary>Raised when machine operational status changes (server push).</summary>
    public event Action<MachineStatusChangedEvent>? OnMachineStatusChanged;

    /// <summary>Raised when job status or produced quantity changes (server push).</summary>
    public event Action<JobStatusChangedEvent>? OnJobStatusChanged;

    /// <summary>Raised when a new alarm is triggered (server push).</summary>
    public event Action<AlarmTriggeredEvent>? OnAlarmTriggered;

    /// <summary>Raised when OEE snapshot values change (server push).</summary>
    public event Action<OeeUpdatedEvent>? OnOeeUpdated;

    /// <summary>Raised after connection state transitions (connect, reconnect, disconnect).</summary>
    public event Action? ConnectionStateChanged;

    /// <summary>Starts the hub connection if not already connected.</summary>
    /// <param name="hubUrl">Absolute hub URL (typically from configuration).</param>
    public async Task StartAsync(string hubUrl)
    {
        await _startGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_connection?.State == HubConnectionState.Connected)
                return;

            await StopInternalAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(hubUrl))
            {
                _logger.LogWarning("SignalR hub URL is empty; skipping hub start.");
                return;
            }

            _negotiatedCookieHeader = await ResolveAuthCookieHeaderAsync().ConfigureAwait(false);

            var cookieSnapshot = _negotiatedCookieHeader;
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl.Trim(), options =>
                {
                    options.HttpMessageHandlerFactory = _ =>
                        new CookieAppendHandler(cookieSnapshot)
                        {
                            InnerHandler = new HttpClientHandler(),
                        };
                })
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers(_connection);

            _connection.Reconnecting += _ =>
            {
                RaiseConnectionStateChanged();
                return Task.CompletedTask;
            };
            _connection.Reconnected += _ =>
            {
                RaiseConnectionStateChanged();
                return Task.CompletedTask;
            };
            _connection.Closed += _ =>
            {
                RaiseConnectionStateChanged();
                return Task.CompletedTask;
            };

            await _connection.StartAsync().ConfigureAwait(false);
            RaiseConnectionStateChanged();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR hub failed to start for URL {HubUrl}", hubUrl);
            await StopInternalAsync().ConfigureAwait(false);
            RaiseConnectionStateChanged();
        }
        finally
        {
            _startGate.Release();
        }
    }

    /// <summary>Stops and disposes the hub connection.</summary>
    public async Task StopAsync()
    {
        await _startGate.WaitAsync().ConfigureAwait(false);
        try
        {
            await StopInternalAsync().ConfigureAwait(false);
            RaiseConnectionStateChanged();
        }
        finally
        {
            _startGate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
        _startGate.Dispose();
    }

    private void RegisterHandlers(HubConnection connection)
    {
        connection.On<MachineStatusChangedEvent>("MachineStatusChanged", evt => OnMachineStatusChanged?.Invoke(evt));
        connection.On<JobStatusChangedEvent>("JobStatusChanged", evt => OnJobStatusChanged?.Invoke(evt));
        connection.On<AlarmTriggeredEvent>("AlarmTriggered", evt => OnAlarmTriggered?.Invoke(evt));
        connection.On<OeeUpdatedEvent>("OeeUpdated", evt => OnOeeUpdated?.Invoke(evt));
    }

    private void RaiseConnectionStateChanged()
    {
        try
        {
            ConnectionStateChanged?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Hub ConnectionStateChanged handler failed.");
        }
    }

    private async Task StopInternalAsync()
    {
        if (_connection is null)
            return;
        try
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "SignalR hub dispose suppressed.");
        }
        finally
        {
            _connection = null;
        }
    }

    private async Task<string?> ResolveAuthCookieHeaderAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request.Cookies.TryGetValue(XMachineAuthDefaults.CookieName, out var direct) == true &&
            !string.IsNullOrEmpty(direct))
            return $"{XMachineAuthDefaults.CookieName}={direct}";

        await using var scope = _scopeFactory.CreateAsyncScope();
        try
        {
            var authStateProvider = scope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();
            var authState = await authStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
            var userIdValue = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdValue, out var userId))
            {
                var stored = _cookieStore.Get(userId);
                if (!string.IsNullOrEmpty(stored))
                    return $"{XMachineAuthDefaults.CookieName}={stored}";
            }
        }
        catch
        {
            // Never block hub negotiation
        }

        var recent = _cookieStore.GetMostRecent();
        return string.IsNullOrEmpty(recent) ? null : $"{XMachineAuthDefaults.CookieName}={recent}";
    }

    /// <summary>Appends a fixed Cookie header for negotiate/WebSocket requests.</summary>
    private sealed class CookieAppendHandler : DelegatingHandler
    {
        private readonly string? _cookieHeader;

        public CookieAppendHandler(string? cookieHeader) => _cookieHeader = cookieHeader;

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_cookieHeader))
                request.Headers.TryAddWithoutValidation("Cookie", _cookieHeader);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
