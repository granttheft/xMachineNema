using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Web.Services;

/// <summary>
/// Loads flat JSON locale files from <c>wwwroot/locales</c> and merges tenant overrides from the operational database.
/// </summary>
public sealed class TranslationService : ITranslationService
{
    private const string LangCookieName = "xm_lang";
    private static readonly HashSet<string> SupportedLanguages = new(StringComparer.OrdinalIgnoreCase)
    {
        "en", "tr", "de", "pl",
    };

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _environment;
    private readonly IDbContextFactory<XMachineDbContext> _dbContextFactory;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<TranslationService> _logger;

    private readonly object _loadGate = new();
    private Dictionary<string, string> _strings = new(StringComparer.Ordinal);
    private bool _initialized;

    public TranslationService(
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment,
        IDbContextFactory<XMachineDbContext> dbContextFactory,
        ICurrentUser currentUser,
        ILogger<TranslationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
        _dbContextFactory = dbContextFactory;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <inheritdoc />
    public string CurrentLanguage => ResolveLanguageCode();

    /// <inheritdoc />
    public string this[string key] => Get(key);

    /// <inheritdoc />
    public string Get(string key, string? fallback = null)
    {
        EnsureLoaded();
        if (_strings.TryGetValue(key, out var value))
            return value;

        return string.IsNullOrEmpty(fallback) ? key : fallback;
    }

    /// <inheritdoc />
    public Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureLoaded();
        return Task.CompletedTask;
    }

    private void EnsureLoaded()
    {
        lock (_loadGate)
        {
            if (_initialized)
                return;

            LoadCore();
            _initialized = true;
        }
    }

    private void LoadCore()
    {
        var lang = ResolveLanguageCode();
        var map = new Dictionary<string, string>(StringComparer.Ordinal);

        try
        {
            var path = Path.Combine(_environment.WebRootPath, "locales", $"{lang}.json");
            if (!File.Exists(path))
            {
                _logger.LogWarning("Locale file missing for language {Language}: {Path}", lang, path);
            }
            else
            {
                var json = File.ReadAllText(path);
                var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (parsed is not null)
                {
                    foreach (var kv in parsed)
                        map[kv.Key] = kv.Value;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load locale JSON for {Language}", lang);
        }

        try
        {
            var tenantId = _currentUser.TenantId;
            if (tenantId is not null)
            {
                using var db = _dbContextFactory.CreateDbContext();
                var rows = db.TenantTranslationOverrides.AsNoTracking()
                    .Where(x =>
                        x.TenantId == tenantId &&
                        x.LanguageCode == lang &&
                        x.Status == EntityStatus.Active)
                    .Select(x => new { x.Key, x.Value })
                    .ToList();

                foreach (var row in rows)
                    map[row.Key] = row.Value;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load translation overrides for tenant {TenantId}", _currentUser.TenantId);
        }

        _strings = map;
    }

    private string ResolveLanguageCode()
    {
        var ctx = _httpContextAccessor.HttpContext;
        var raw = ctx?.Request.Cookies[LangCookieName];
        if (!string.IsNullOrWhiteSpace(raw) && SupportedLanguages.Contains(raw.Trim()))
            return raw.Trim().ToLowerInvariant();

        return "en";
    }
}
