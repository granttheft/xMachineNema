using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XMachine.Persistence.Operational;

public sealed class XMachineDbContextDesignTimeFactory : IDesignTimeDbContextFactory<XMachineDbContext>
{
    public XMachineDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("XMACHINE_OPERATIONAL_DB")
            ?? "Host=localhost;Port=5432;Database=xmachine;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<XMachineDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new XMachineDbContext(optionsBuilder.Options);
    }
}

