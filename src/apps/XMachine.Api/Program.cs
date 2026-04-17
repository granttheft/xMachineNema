using Microsoft.EntityFrameworkCore;
using XMachine.Api.Integration;
using XMachine.Api.Mes;
using XMachine.Api.Quality;
using XMachine.Api.Eventing;
using XMachine.Connectors.Runtime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var operationalDbCs = builder.Configuration.GetConnectionString("XMachineOperationalDb");
if (string.IsNullOrWhiteSpace(operationalDbCs))
{
    throw new InvalidOperationException(
        "Missing connection string: ConnectionStrings:XMachineOperationalDb");
}

builder.Services.AddDbContext<XMachine.Persistence.Operational.XMachineDbContext>(options =>
    options.UseNpgsql(operationalDbCs));

builder.Services.AddConnectorRuntime(registry =>
{
    foreach (var code in new[] { "opcua", "s7", "modbus_tcp", "sap", "rest" })
        registry.Register(new PlaceholderConnectorFactory(code));
});

builder.Services.AddHostedService<XMachine.Api.Development.DevSeedHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }))
    .WithName("Live");

app.MapGet("/health/ready", async (XMachine.Persistence.Operational.XMachineDbContext db, CancellationToken ct) =>
{
    var canConnect = await db.Database.CanConnectAsync(ct);
    return canConnect
        ? Results.Ok(new { status = "ready" })
        : Results.Problem("Database is not reachable.");
}).WithName("Ready");

app.MapIntegrationEndpoints();
app.MapMesEndpoints();
app.MapQualityEndpoints();
app.MapEventingEndpoints();

app.Run();
