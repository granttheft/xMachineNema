namespace XMachine.Module.Auth.Security;

/// <summary>
/// Role codes stored on <see cref="Domain.Role.Code"/> and emitted as role claims.
/// Policy names match these codes for simple <c>RequireRole</c> checks.
/// </summary>
public static class ApplicationRoles
{
    public const string SuperAdmin = nameof(SuperAdmin);
    public const string TenantAdmin = nameof(TenantAdmin);
    public const string SiteAdmin = nameof(SiteAdmin);
    public const string ProductionManager = nameof(ProductionManager);
    public const string Quality = nameof(Quality);
    public const string Maintenance = nameof(Maintenance);
    public const string Operator = nameof(Operator);

    public static readonly IReadOnlyList<string> All =
    [
        SuperAdmin,
        TenantAdmin,
        SiteAdmin,
        ProductionManager,
        Quality,
        Maintenance,
        Operator,
    ];
}
