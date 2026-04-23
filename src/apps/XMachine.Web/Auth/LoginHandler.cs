using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Web.Auth;

/// <summary>Cookie sign-in via full HTTP POST (avoids Blazor interactive response limitations).</summary>
public static class LoginHandler
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (
                HttpContext ctx,
                XMachineDbContext db,
                IOptions<DevAuthOptions> devAuthOptions,
                IWebHostEnvironment env,
                CancellationToken cancellationToken) =>
            {
                var form = await ctx.Request.ReadFormAsync(cancellationToken).ConfigureAwait(false);
                var username = form["username"].ToString().Trim();
                var password = form["password"].ToString();
                var returnUrl = form["returnUrl"].ToString();

                var devAuth = devAuthOptions.Value;
                if (!devAuth.Enabled || !env.IsDevelopment())
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await ctx.Response.WriteAsync("Sign-in not available.", cancellationToken).ConfigureAwait(false);
                    return;
                }

                if (string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(devAuth.SharedPassword) ||
                    !string.Equals(password, devAuth.SharedPassword, StringComparison.Ordinal))
                {
                    ctx.Response.Redirect(BuildLoginErrorUrl(returnUrl));
                    return;
                }

                var user = await db.UserAccounts.AsNoTracking()
                    .Where(u => u.Username == username && u.Status == EntityStatus.Active)
                    .Select(u => new { u.Id, u.TenantId, u.Username, u.DisplayName })
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (user is null)
                {
                    ctx.Response.Redirect(BuildLoginErrorUrl(returnUrl));
                    return;
                }

                var roleCodes = await (
                        from ura in db.UserRoleAssignments.AsNoTracking()
                        join r in db.Roles.AsNoTracking() on ura.RoleId equals r.Id
                        where ura.UserAccountId == user.Id
                              && ura.Status == EntityStatus.Active
                              && r.Status == EntityStatus.Active
                        select r.Code)
                    .Distinct()
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

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

                // Capture cookie value for Blazor circuit access
                var cookieStore = ctx.RequestServices.GetRequiredService<UserCookieStore>();
                var capturedUserId = user.Id;

                ctx.Response.OnStarting(() =>
                {
                    var setCookieHeaders = ctx.Response.Headers["Set-Cookie"];
                    foreach (var header in (IEnumerable<string>)setCookieHeaders)
                    {
                        if (header is null) continue;
                        var prefix = XMachineAuthDefaults.CookieName + "=";
                        if (header.StartsWith(prefix, StringComparison.Ordinal))
                        {
                            var rawValue = header.Split(';')[0].Substring(prefix.Length);
                            if (!string.IsNullOrEmpty(rawValue))
                                cookieStore.Set(capturedUserId, rawValue);
                            break;
                        }
                    }

                    return Task.CompletedTask;
                });

                await ctx.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true,
                    }).ConfigureAwait(false);

                var target = PickSafeReturnUrl(returnUrl);
                ctx.Response.Redirect(target);
            })
            .AllowAnonymous()
            .DisableAntiforgery();
    }

    private static string BuildLoginErrorUrl(string? returnUrl)
    {
        var q = new Dictionary<string, string?> { ["error"] = "invalid" };
        if (!string.IsNullOrWhiteSpace(returnUrl))
            q["returnUrl"] = returnUrl;
        return QueryHelpers.AddQueryString("/login", q);
    }

    private static string PickSafeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) ||
            !returnUrl.StartsWith("/", StringComparison.Ordinal) ||
            returnUrl.StartsWith("//", StringComparison.Ordinal))
            return "/dashboard";
        return returnUrl;
    }
}
