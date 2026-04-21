using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Persistence.Operational;
using XMachine.Web.Auth;
using XMachine.Web.Components;
using XMachine.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<UserCookieStore>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, HttpContextAuthenticationStateProvider>();

var authKeysPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "..", ".xmachine-auth-keys"));
Directory.CreateDirectory(authKeysPath);
builder.Services.AddDataProtection()
    .SetApplicationName(XMachineAuthDefaults.DataProtectionApplicationName)
    .PersistKeysToFileSystem(new DirectoryInfo(authKeysPath));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = XMachineAuthDefaults.CookieName;
        options.LoginPath = "/login";
    });

builder.Services.AddAuthorization(o => XMachineAuthorizationPolicies.AddPolicies(o));

builder.Services.AddScoped<XMachine.Module.Auth.Security.ICurrentUser,
    XMachine.Module.Auth.Security.CurrentUser>();

var operationalDbCs = builder.Configuration.GetConnectionString("XMachineOperationalDb");
if (string.IsNullOrWhiteSpace(operationalDbCs))
{
    throw new InvalidOperationException(
        "Missing connection string: ConnectionStrings:XMachineOperationalDb");
}

builder.Services.AddDbContext<XMachineDbContext>(options =>
    options.UseNpgsql(operationalDbCs));

builder.Services.Configure<DevAuthOptions>(builder.Configuration.GetSection(DevAuthOptions.SectionPath));

builder.Services.AddTransient<ForwardingAuthCookieHandler>();

var apiBaseUrl = ApiBaseUrlResolver.Resolve(builder.Configuration);
builder.Services.AddSingleton<ApiConnectivityState>();
builder.Services.AddHttpClient(nameof(ApiLiveProbeHostedService), client => client.Timeout = TimeSpan.FromSeconds(8));
builder.Services.AddHostedService<ApiLiveProbeHostedService>();

builder.Services.AddHttpClient<XMachineApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
}).AddHttpMessageHandler<ForwardingAuthCookieHandler>();

var app = builder.Build();

var resolvedApi = ApiBaseUrlResolver.Resolve(app.Configuration);
app.Logger.LogInformation(
    "XMachine.Web → API {ApiBaseUrl} (official live health: {LiveUrl}). Configure {ConfigKey} (empty uses default {Default}).",
    resolvedApi,
    resolvedApi + "/health/live",
    ApiBaseUrlResolver.ConfigKey,
    ApiBaseUrlResolver.DefaultLocalApi);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// After authentication so HttpContext.User has NameIdentifier for cookie capture.
app.UseMiddleware<CookieCaptureMiddleware>();

app.UseAntiforgery();

app.MapGet("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
}).AllowAnonymous();

app.MapStaticAssets();
app.MapLoginEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
