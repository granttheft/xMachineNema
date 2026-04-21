using XMachine.Web.Components.Shared;

namespace XMachine.Web.Services;

/// <summary>Short English labels for domain enum names from the API (no i18n layer).</summary>
public static class UiLabels
{
    public static string EntityStatus(string? value) => value switch
    {
        "Active" => "Active",
        "Inactive" => "Inactive",
        "Archived" => "Archived",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone EntityStatusTone(string? value) => value switch
    {
        "Active" => StatusBadgeTone.Success,
        "Inactive" => StatusBadgeTone.Warning,
        "Archived" => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string ProductionOrderStatus(string? value) => value switch
    {
        "Draft" => "Draft",
        "Released" => "Released",
        "InProgress" => "In progress",
        "Completed" => "Completed",
        "Cancelled" => "Cancelled",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone ProductionOrderStatusTone(string? value) => value switch
    {
        "Completed" => StatusBadgeTone.Success,
        "Cancelled" => StatusBadgeTone.Warning,
        "InProgress" => StatusBadgeTone.Info,
        "Released" => StatusBadgeTone.Info,
        "Draft" => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string LotBatchStatus(string? value) => value switch
    {
        "Planned" => "Planned",
        "Active" => "Active",
        "Closed" => "Closed",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone LotBatchStatusTone(string? value) => value switch
    {
        "Active" => StatusBadgeTone.Info,
        "Closed" => StatusBadgeTone.Success,
        "Planned" => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string WorkShiftLifecycle(string? value) => value switch
    {
        "Planned" => "Planned",
        "Open" => "Open",
        "Closed" => "Closed",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone WorkShiftLifecycleTone(string? value) => value switch
    {
        "Open" => StatusBadgeTone.Info,
        "Closed" => StatusBadgeTone.Success,
        "Planned" => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string AlarmSeverity(string? value) => value switch
    {
        "Info" => "Info",
        "Warning" => "Warning",
        "Error" => "Error",
        "Critical" => "Critical",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone AlarmSeverityTone(string? value) => value switch
    {
        "Critical" => StatusBadgeTone.Danger,
        "Error" => StatusBadgeTone.Danger,
        "Warning" => StatusBadgeTone.Warning,
        "Info" => StatusBadgeTone.Info,
        _ => StatusBadgeTone.Neutral,
    };

    public static string AlarmLifecycle(string? value) => value switch
    {
        "Active" => "Active",
        "Acknowledged" => "Acknowledged",
        "Cleared" => "Cleared",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone AlarmLifecycleTone(string? value) => value switch
    {
        "Active" => StatusBadgeTone.Danger,
        "Acknowledged" => StatusBadgeTone.Warning,
        "Cleared" => StatusBadgeTone.Success,
        _ => StatusBadgeTone.Neutral,
    };

    public static string OeePeriodType(string? value) => value switch
    {
        "Hour" => "Hour",
        "Shift" => "Shift",
        "Day" => "Day",
        "Week" => "Week",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static string QualityCheckStatus(string? value) => value switch
    {
        "Pending" => "Pending",
        "InProgress" => "In progress",
        "Passed" => "Passed",
        "Failed" => "Failed",
        "Waived" => "Waived",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone QualityCheckStatusTone(string? value) => value switch
    {
        "Passed" => StatusBadgeTone.Success,
        "Failed" => StatusBadgeTone.Danger,
        "Waived" => StatusBadgeTone.Warning,
        "InProgress" => StatusBadgeTone.Info,
        _ => StatusBadgeTone.Neutral,
    };

    public static string NonconformanceSeverity(string? value) => value switch
    {
        "Minor" => "Minor",
        "Major" => "Major",
        "Critical" => "Critical",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone NonconformanceSeverityTone(string? value) => value switch
    {
        "Critical" => StatusBadgeTone.Danger,
        "Major" => StatusBadgeTone.Warning,
        _ => StatusBadgeTone.Neutral,
    };

    public static string NonconformanceStatus(string? value) => value switch
    {
        "Open" => "Open",
        "UnderReview" => "Under review",
        "Closed" => "Closed",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone NonconformanceStatusTone(string? value) => value switch
    {
        "Open" => StatusBadgeTone.Warning,
        "UnderReview" => StatusBadgeTone.Info,
        "Closed" => StatusBadgeTone.Success,
        _ => StatusBadgeTone.Neutral,
    };

    public static string WorkflowInstanceState(string? value) => value switch
    {
        "Draft" => "Draft",
        "InProgress" => "In progress",
        "Approved" => "Approved",
        "Rejected" => "Rejected",
        "Cancelled" => "Cancelled",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone WorkflowInstanceStateTone(string? value) => value switch
    {
        "Approved" => StatusBadgeTone.Success,
        "Rejected" => StatusBadgeTone.Danger,
        "Cancelled" => StatusBadgeTone.Warning,
        "InProgress" => StatusBadgeTone.Info,
        _ => StatusBadgeTone.Neutral,
    };

    public static string ConnectorDirection(string? value) => value switch
    {
        "Inbound" => "Inbound",
        "Outbound" => "Outbound",
        "Bidirectional" => "Bidirectional",
        null or "" => "—",
        _ => string.IsNullOrWhiteSpace(value) ? "—" : value!,
    };

    public static StatusBadgeTone ConnectorDirectionTone(string? value) => value switch
    {
        "Bidirectional" => StatusBadgeTone.Info,
        _ => StatusBadgeTone.Neutral,
    };

    /// <summary>Turns snake_case API strings into Title Case for display.</summary>
    public static string HumanizeToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "—";
        var s = value.Trim();
        if (s.Length == 1)
            return char.ToUpperInvariant(s[0]).ToString();
        if (!s.Contains('_', StringComparison.Ordinal))
            return char.ToUpperInvariant(s[0]) + s[1..];
        return string.Join(' ', s.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(static p => p.Length == 0 ? p : char.ToUpperInvariant(p[0]) + p[1..]));
    }
}
