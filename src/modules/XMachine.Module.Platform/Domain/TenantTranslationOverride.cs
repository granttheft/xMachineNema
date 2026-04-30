using XMachine.SharedKernel;
using XMachine.SharedKernel.Entities;

namespace XMachine.Module.Platform.Domain;

/// <summary>
/// Per-tenant translation string override for a given language and logical key.
/// </summary>
public sealed class TenantTranslationOverride : TenantAuditableEntity
{
    /// <summary>ISO 639-1 language code (e.g. en, tr, de, pl).</summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>Translation key, e.g. <c>nav.dashboard</c>.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Translated text for this tenant and language.</summary>
    public string Value { get; set; } = string.Empty;

    public EntityStatus Status { get; set; } = EntityStatus.Active;
}
