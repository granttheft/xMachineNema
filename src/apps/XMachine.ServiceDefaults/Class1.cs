namespace XMachine.ServiceDefaults;

public static class XMachineDefaults
{
    public static string NormalizeServiceName(string rawName)
        => rawName.Replace('.', '-').Replace('_', '-').ToLowerInvariant();
}
