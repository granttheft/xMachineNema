using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using XMachine.Module.Auth.Domain;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Web.Auth;

/// <summary>
/// Looks up <see cref="UserAccount"/> + roles, validates a shared dev password when enabled,
/// then issues the ASP.NET Core authentication cookie.
/// </summary>
public sealed class DevelopmentUserSignInService : IApplicationUserSignInService
{
    private readonly XMachineDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;
    private readonly DevAuthOptions _devAuth;

    public DevelopmentUserSignInService(
        XMachineDbContext db,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment env,
        IOptions<DevAuthOptions> devAuth)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _env = env;
        _devAuth = devAuth.Value;
    }

    public async Task<ApplicationUserSignInResult> SignInAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var http = _httpContextAccessor.HttpContext;
        if (http is null)
            return new ApplicationUserSignInResult(false, "No active HTTP context.");

        var normalized = username.Trim();
        if (normalized.Length == 0)
            return new ApplicationUserSignInResult(false, "Enter a username.");

        if (!_devAuth.Enabled)
            return new ApplicationUserSignInResult(false, "Development sign-in is disabled (XMachine:DevAuth:Enabled=false).");

        if (!_env.IsDevelopment())
            return new ApplicationUserSignInResult(false, "Password sign-in is only enabled in Development for this skeleton.");

        if (string.IsNullOrEmpty(_devAuth.SharedPassword) || !string.Equals(password, _devAuth.SharedPassword, StringComparison.Ordinal))
            return new ApplicationUserSignInResult(false, "Invalid username or password.");

        var user = await _db.UserAccounts.AsNoTracking()
            .Where(u => u.Username == normalized && u.Status == EntityStatus.Active)
            .Select(u => new { u.Id, u.TenantId, u.Username, u.DisplayName })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return new ApplicationUserSignInResult(false, "Invalid username or password.");

        var roleCodes = await (
                from ura in _db.UserRoleAssignments.AsNoTracking()
                join r in _db.Roles.AsNoTracking() on ura.RoleId equals r.Id
                where ura.UserAccountId == user.Id
                      && ura.Status == EntityStatus.Active
                      && r.Status == EntityStatus.Active
                select r.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new("preferred_username", user.Username),
            new(XMachineClaims.TenantId, user.TenantId.ToString()),
        };

        foreach (var code in roleCodes)
            claims.Add(new Claim(ClaimTypes.Role, code));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });

        return new ApplicationUserSignInResult(true, null);
    }
}
