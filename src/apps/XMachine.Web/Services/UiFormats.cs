namespace XMachine.Web.Services;

/// <summary>Consistent date/time display for operations screens (server-rendered, UTC-labelled).</summary>
public static class UiFormats
{
    public const string UtcMinuteFormat = "yyyy-MM-dd HH:mm";

    public static string UtcMinute(DateTimeOffset value) =>
        $"{value.UtcDateTime.ToString(UtcMinuteFormat)} UTC";

    public static string UtcMinute(DateTimeOffset? value, string empty = "—") =>
        value is { } v ? UtcMinute(v) : empty;

    public static string UtcDate(DateTimeOffset value) =>
        $"{value.UtcDateTime:yyyy-MM-dd} UTC";

    public static string UtcDate(DateTimeOffset? value, string empty = "—") =>
        value is { } v ? UtcDate(v) : empty;
}
