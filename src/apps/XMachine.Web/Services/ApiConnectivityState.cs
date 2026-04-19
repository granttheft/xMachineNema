namespace XMachine.Web.Services;



/// <summary>

/// Result of a one-shot <c>/health/live</c> probe after Web startup (for shell-level visibility only).

/// </summary>

public sealed class ApiConnectivityState

{

    private int _status;



    /// <summary>0 = not probed yet, 1 = reachable, 2 = unreachable.</summary>

    public bool HasResult => Volatile.Read(ref _status) != 0;



    public bool IsReachable => Volatile.Read(ref _status) == 1;



    public string ResolvedApiBaseUrl { get; private set; } = "";



    public void SetResult(bool reachable, string resolvedApiBaseUrl)

    {

        ResolvedApiBaseUrl = resolvedApiBaseUrl;

        Interlocked.Exchange(ref _status, reachable ? 1 : 2);

    }

}


