using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XMachine.AppHost;

internal static class Program
{
    private const string DefaultApiUrl = "http://localhost:5090";
    private const string DefaultWebUrl = "http://localhost:5197";

    public static async Task<int> Main()
    {
        var appsDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var apiProject = Path.Combine(appsDirectory, "XMachine.Api", "XMachine.Api.csproj");
        var webProject = Path.Combine(appsDirectory, "XMachine.Web", "XMachine.Web.csproj");

        if (!File.Exists(apiProject) || !File.Exists(webProject))
        {
            Console.Error.WriteLine("Could not locate sibling projects from AppHost output directory.");
            Console.Error.WriteLine($"Expected: {apiProject}");
            Console.Error.WriteLine($"Expected: {webProject}");
            return 1;
        }

        Console.WriteLine("xMachine AppHost (local dev)");
        Console.WriteLine($"  Api  project: {apiProject}");
        Console.WriteLine($"  Web  project: {webProject}");
        Console.WriteLine($"  Api  URL:    {DefaultApiUrl}");
        Console.WriteLine($"  Web  URL:    {DefaultWebUrl}");
        Console.WriteLine("Press Ctrl+C to stop both processes.");
        Console.WriteLine();

        var children = new List<Process>();

        void KillChildren()
        {
            foreach (var p in children)
            {
                try
                {
                    if (p.HasExited)
                        continue;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        p.Kill(entireProcessTree: true);
                    else
                        p.Kill();
                }
                catch
                {
                    // ignore
                }
            }
        }

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            KillChildren();
        };

        try
        {
            children.Add(StartDotnet(apiProject, new Dictionary<string, string>
            {
                ["ASPNETCORE_URLS"] = DefaultApiUrl,
            }));

            if (!await WaitForApiLiveAsync(DefaultApiUrl, TimeSpan.FromSeconds(90), CancellationToken.None)
                .ConfigureAwait(false))
            {
                Console.Error.WriteLine("Api did not become healthy in time. Check PostgreSQL, connection string, and migrations.");
                return 2;
            }

            Console.WriteLine("Api is up. Starting Web…");
            children.Add(StartDotnet(webProject, new Dictionary<string, string>
            {
                ["ASPNETCORE_URLS"] = DefaultWebUrl,
                ["XMachine__Api__BaseUrl"] = DefaultApiUrl,
            }));

            await Task.WhenAll(children.Select(p => p.WaitForExitAsync())).ConfigureAwait(false);
            return children.Any(p => p.ExitCode != 0) ? 3 : 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 4;
        }
        finally
        {
            KillChildren();
        }
    }

    private static Process StartDotnet(string projectPath, IReadOnlyDictionary<string, string> env)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\"",
            UseShellExecute = false,
            CreateNoWindow = false,
        };

        foreach (System.Collections.DictionaryEntry kv in Environment.GetEnvironmentVariables())
            psi.Environment[(string)kv.Key!] = kv.Value?.ToString() ?? string.Empty;

        foreach (var (k, v) in env)
            psi.Environment[k] = v;

        var p = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start dotnet process.");
        return p;
    }

    private static async Task<bool> WaitForApiLiveAsync(string baseUrl, TimeSpan timeout, CancellationToken ct)
    {
        var root = baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/";
        using var client = new HttpClient { BaseAddress = new Uri(root) };
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var r = await client.GetAsync("health/live", ct).ConfigureAwait(false);
                if (r.IsSuccessStatusCode)
                    return true;
            }
            catch
            {
                // retry
            }

            await Task.Delay(500, ct).ConfigureAwait(false);
        }

        return false;
    }
}
