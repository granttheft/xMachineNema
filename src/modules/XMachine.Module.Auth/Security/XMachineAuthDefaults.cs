namespace XMachine.Module.Auth.Security;

/// <summary>Shared cookie auth settings for Web + API (same DP application name + cookie name).</summary>
public static class XMachineAuthDefaults
{
    public const string DataProtectionApplicationName = "XMachine";
    public const string CookieName = "xmachine.auth";
}
