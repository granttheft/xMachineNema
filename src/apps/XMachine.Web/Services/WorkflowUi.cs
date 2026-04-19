namespace XMachine.Web.Services;

internal static class WorkflowUi
{
    public static string InstanceStateLabel(int value) => value switch
    {
        1 => "Draft",
        2 => "In progress",
        3 => "Approved",
        4 => "Rejected",
        5 => "Cancelled",
        _ => value.ToString(),
    };

    public static string EntityStatusLabel(int value) => value switch
    {
        1 => "Active",
        2 => "Inactive",
        3 => "Archived",
        _ => value.ToString(),
    };
}
