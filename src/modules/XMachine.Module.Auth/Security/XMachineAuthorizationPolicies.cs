using Microsoft.AspNetCore.Authorization;

namespace XMachine.Module.Auth.Security;

public static class XMachineAuthorizationPolicies
{
    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(AuthPolicies.AdminArea, p => p.RequireRole(
            ApplicationRoles.SuperAdmin,
            ApplicationRoles.TenantAdmin));

        foreach (var code in ApplicationRoles.All)
            options.AddPolicy(code, policy => policy.RequireRole(code));
    }
}
