namespace XMachine.Web.Auth;

/// <summary>Development-only password gate. Replace with real credential validation later.</summary>
public sealed class DevAuthOptions
{
    public const string SectionPath = "XMachine:DevAuth";

    public bool Enabled { get; set; }
    public string SharedPassword { get; set; } = "";
}
