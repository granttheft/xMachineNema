using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using XMachine.Api.Integration;
using XMachine.Api.Mes;
using XMachine.Api.Quality;
using XMachine.Api.Eventing;
using XMachine.Api.Platform;
using XMachine.Api.Commercial;
using XMachine.Api.Workflow;
using XMachine.Connectors.Runtime;
using XMachine.Module.Auth.Security;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var authKeysPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "..", ".xmachine-auth-keys"));
Directory.CreateDirectory(authKeysPath);
builder.Services.AddDataProtection()
    .SetApplicationName(XMachineAuthDefaults.DataProtectionApplicationName)
    .PersistKeysToFileSystem(new DirectoryInfo(authKeysPath));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = XMachineAuthDefaults.CookieName;
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(o => XMachineAuthorizationPolicies.AddPolicies(o));

var operationalDbCs = builder.Configuration.GetConnectionString("XMachineOperationalDb");
if (string.IsNullOrWhiteSpace(operationalDbCs))
{
    throw new InvalidOperationException(
        "Missing connection string: ConnectionStrings:XMachineOperationalDb");
}

builder.Services.AddDbContext<XMachine.Persistence.Operational.XMachineDbContext>(options =>
    options.UseNpgsql(operationalDbCs));

builder.Services.AddHostedService<XMachine.Api.Development.DatabaseMigrationHostedService>();
builder.Services.AddConnectorRuntime(registry =>
{
    foreach (var code in new[] { "opcua", "s7", "modbus_tcp", "sap", "rest" })
        registry.Register(new PlaceholderConnectorFactory(code));
});

builder.Services.AddHostedService<XMachine.Api.Development.DevSeedHostedService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    live = "/health/live",
    ready = "/health/ready",
    devSummary = "/health/dev-summary",
})).WithName("Health");

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }))
    .WithName("Live");

app.MapGet("/health/ready", async (XMachine.Persistence.Operational.XMachineDbContext db, CancellationToken ct) =>
{
    var canConnect = await db.Database.CanConnectAsync(ct);
    return canConnect
        ? Results.Ok(new { status = "ready" })
        : Results.Problem("Database is not reachable.");
}).WithName("Ready");

app.MapGet("/health/dev-summary", (IConfiguration cfg, IHostEnvironment env) =>
{
    if (!env.IsDevelopment())
        return Results.NotFound();
    return Results.Ok(new
    {
        hasOperationalDb = !string.IsNullOrWhiteSpace(cfg.GetConnectionString("XMachineOperationalDb")),
        migrateOnStartup = cfg.GetValue("XMachine:Database:MigrateOnStartup", false),
        seedEnabled = cfg.GetValue("XMachine:Seed:Enabled", false),
    });
}).WithName("DevSummary");

app.MapIntegrationEndpoints();
app.MapMesEndpoints();
app.MapQualityEndpoints();
app.MapEventingEndpoints();
app.MapPlatformEndpoints();
app.MapCommercialEndpoints();
app.MapWorkflowEndpoints();

app.Run();
