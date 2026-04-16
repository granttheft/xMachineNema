using Microsoft.Extensions.DependencyInjection;
using XMachine.Connectors.Abstractions;

namespace XMachine.Connectors.Runtime;

public static class ConnectorRuntimeServiceCollectionExtensions
{
    /// <summary>Registers registry, orchestrator, and placeholder-friendly health provider.</summary>
    public static IServiceCollection AddConnectorRuntime(
        this IServiceCollection services,
        Action<IConnectorRegistry>? configureRegistry = null)
    {
        services.AddSingleton<IConnectorRegistry>(_ =>
        {
            var registry = new ConnectorRegistry();
            configureRegistry?.Invoke(registry);
            return registry;
        });

        services.AddSingleton<ConnectorOrchestrator>();
        services.AddSingleton<IConnectorHealthProvider, RuntimeConnectorHealthProvider>();
        return services;
    }
}
