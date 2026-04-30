namespace XMachine.Web.Services;

/// <summary>Row from GET /api/admin/languages.</summary>
public sealed record LanguageSummaryRowDto(string Code, string Name, int OverrideCount);

/// <summary>Row from GET /api/admin/languages/{code}/overrides.</summary>
public sealed record TranslationOverrideRowDto(Guid Id, string Key, string Value, string LanguageCode);

/// <summary>POST /api/admin/languages/{code}/overrides request body.</summary>
public sealed record UpsertTranslationOverrideRequestDto(string Key, string Value);

/// <summary>POST /api/admin/languages/{code}/overrides response.</summary>
public sealed record UpsertTranslationOverrideResponseDto(Guid Id, string Key, string Value);
