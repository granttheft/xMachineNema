using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using XMachine.Persistence.Operational;

namespace XMachine.Api.Development;

/// <summary>
/// Applies EF Core migrations at API startup when <c>XMachine:Database:MigrateOnStartup</c> is true.
/// Runs before <see cref="DevSeedHostedService"/> (register this service first).
/// </summary>
internal sealed class DatabaseMigrationHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseMigrationHostedService> _logger;

    public DatabaseMigrationHostedService(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger<DatabaseMigrationHostedService> logger)
    {
        _services = services;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_configuration.GetValue("XMachine:Database:MigrateOnStartup", false))
        {
            _logger.LogInformation(
                "Startup migrations skipped (XMachine:Database:MigrateOnStartup=false). Apply manually: dotnet ef database update --project src/building-blocks/XMachine.Persistence/XMachine.Persistence.csproj --startup-project src/apps/XMachine.Api/XMachine.Api.csproj");
            return;
        }

        await using var scope = _services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<XMachineDbContext>();
        try
        {
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Database migrations applied.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Database migration failed. Verify PostgreSQL is running and ConnectionStrings:XMachineOperationalDb is correct.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
