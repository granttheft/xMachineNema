using XMachine.Web.Components.Shared;

namespace XMachine.Web.Services;

/// <summary>Short English labels for numeric domain codes shown in the Blazor UI (no i18n layer).</summary>
public static class UiLabels
{
    public static string EntityStatus(int value) => value switch
    {
        1 => "Active",
        2 => "Inactive",
        3 => "Archived",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone EntityStatusTone(int value) => value switch
    {
        1 => StatusBadgeTone.Success,
        2 => StatusBadgeTone.Warning,
        3 => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string ProductionOrderStatus(int value) => value switch
    {
        1 => "Draft",
        2 => "Released",
        3 => "In progress",
        4 => "Completed",
        5 => "Cancelled",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone ProductionOrderStatusTone(int value) => value switch
    {
        4 => StatusBadgeTone.Success,
        5 => StatusBadgeTone.Warning,
        3 => StatusBadgeTone.Info,
        2 => StatusBadgeTone.Info,
        1 => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string LotBatchStatus(int value) => value switch
    {
        1 => "Planned",
        2 => "Active",
        3 => "Closed",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone LotBatchStatusTone(int value) => value switch
    {
        2 => StatusBadgeTone.Info,
        3 => StatusBadgeTone.Success,
        1 => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string WorkShiftLifecycle(int value) => value switch
    {
        1 => "Planned",
        2 => "Open",
        3 => "Closed",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone WorkShiftLifecycleTone(int value) => value switch
    {
        2 => StatusBadgeTone.Info,
        3 => StatusBadgeTone.Success,
        1 => StatusBadgeTone.Neutral,
        _ => StatusBadgeTone.Neutral,
    };

    public static string AlarmSeverity(int value) => value switch
    {
        1 => "Info",
        2 => "Warning",
        3 => "Error",
        4 => "Critical",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone AlarmSeverityTone(int value) => value switch
    {
        4 => StatusBadgeTone.Danger,
        3 => StatusBadgeTone.Danger,
        2 => StatusBadgeTone.Warning,
        1 => StatusBadgeTone.Info,
        _ => StatusBadgeTone.Neutral,
    };

    public static string AlarmLifecycle(int value) => value switch
    {
        1 => "Active",
        2 => "Acknowledged",
        3 => "Cleared",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone AlarmLifecycleTone(int value) => value switch
    {
        1 => StatusBadgeTone.Danger,
        2 => StatusBadgeTone.Warning,
        3 => StatusBadgeTone.Success,
        _ => StatusBadgeTone.Neutral,
    };

    public static string OeePeriodType(int value) => value switch
    {
        1 => "Hour",
        2 => "Shift",
        3 => "Day",
        4 => "Week",
        _ => $"Unknown ({value})",
    };

    public static string QualityCheckStatus(int value) => value switch
    {
        1 => "Pending",
        2 => "In progress",
        3 => "Passed",
        4 => "Failed",
        5 => "Waived",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone QualityCheckStatusTone(int value) => value switch
    {
        3 => StatusBadgeTone.Success,
        4 => StatusBadgeTone.Danger,
        5 => StatusBadgeTone.Warning,
        2 => StatusBadgeTone.Info,
        _ => StatusBadgeTone.Neutral,
    };

    public static string NonconformanceSeverity(int value) => value switch
    {
        1 => "Minor",
        2 => "Major",
        3 => "Critical",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone NonconformanceSeverityTone(int value) => value switch
    {
        3 => StatusBadgeTone.Danger,
        2 => StatusBadgeTone.Warning,
        _ => StatusBadgeTone.Neutral,
    };

    public static string NonconformanceStatus(int value) => value switch
    {
        1 => "Open",
        2 => "Under review",
        3 => "Closed",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone NonconformanceStatusTone(int value) => value switch
    {
        1 => StatusBadgeTone.Warning,
        2 => StatusBadgeTone.Info,
        3 => StatusBadgeTone.Success,
        _ => StatusBadgeTone.Neutral,
    };

    public static string WorkflowInstanceState(int value) => value switch
    {
        1 => "Draft",
        2 => "In progress",
        3 => "Approved",
        4 => "Rejected",
        5 => "Cancelled",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone WorkflowInstanceStateTone(int value) => value switch
    {
        3 => StatusBadgeTone.Success,
        4 => StatusBadgeTone.Danger,
        5 => StatusBadgeTone.Warning,
        2 => StatusBadgeTone.Info,
        _ => StatusBadgeTone.Neutral,
    };

    public static string ConnectorDirection(int value) => value switch
    {
        1 => "Inbound",
        2 => "Outbound",
        3 => "Bidirectional",
        _ => $"Unknown ({value})",
    };

    public static StatusBadgeTone ConnectorDirectionTone(int value) => value switch
    {
        3 => StatusBadgeTone.Info,
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
