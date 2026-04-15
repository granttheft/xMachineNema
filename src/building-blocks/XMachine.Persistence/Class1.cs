namespace XMachine.Persistence;

public static class DevelopmentSeed
{
    public static readonly DevelopmentSeedPlan MinimalPlan = new(
        TenantCount: 1,
        SiteCount: 1,
        LineCount: 2,
        MachineCount: 4,
        IncludesExampleConnectors: true,
        IncludesExampleOrders: true
    );
}

public sealed record DevelopmentSeedPlan(
    int TenantCount,
    int SiteCount,
    int LineCount,
    int MachineCount,
    bool IncludesExampleConnectors,
    bool IncludesExampleOrders
);
