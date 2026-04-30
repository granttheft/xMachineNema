namespace XMachine.Web.Services;

/// <summary>
/// Resolves UI strings for the current request language, using JSON locale files and optional DB overrides.
/// </summary>
public interface ITranslationService
{
    /// <summary>Returns the translation for <paramref name="key"/>; falls back to <paramref name="fallback"/> or <paramref name="key"/>.</summary>
    string Get(string key, string? fallback = null);

    /// <summary>Shorthand for <see cref="Get"/> with no explicit fallback.</summary>
    string this[string key] { get; }

    /// <summary>Active BCP 47 / ISO language code (en, tr, de, pl).</summary>
    string CurrentLanguage { get; }

    /// <summary>Ensures locale JSON and tenant overrides are loaded (call from layout once per circuit).</summary>
    Task EnsureInitializedAsync(CancellationToken cancellationToken = default);
}
