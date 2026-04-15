namespace XMachine.Persistence;

public static class DevelopmentSeed
{
    public static readonly DevelopmentSeedPlan MinimalPlan = new(
        TenantCount: 1,
        EnterpriseCount: 1,
        SiteCount: 1,
        LineCount: 2,
        MachineCount: 4,
        IncludesAuthSeed: true,
        IncludesCommercialSeed: true
    );
}

public sealed record DevelopmentSeedPlan(
    int TenantCount,
    int EnterpriseCount,
    int SiteCount,
    int LineCount,
    int MachineCount,
    bool IncludesAuthSeed,
    bool IncludesCommercialSeed
);
