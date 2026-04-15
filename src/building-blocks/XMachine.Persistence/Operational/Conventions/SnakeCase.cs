namespace XMachine.Persistence.Operational.Conventions;

internal static class SnakeCase
{
    public static string Table(string name) => name;

    public static string Column(string name) => name;
}

