namespace XMachine.Web.Services;



/// <summary>

/// Resolves the XMachine.Api base URL from <c>XMachine:Api:BaseUrl</c> (empty → local default).

/// </summary>

public static class ApiBaseUrlResolver

{

    public const string ConfigKey = "XMachine:Api:BaseUrl";

    public const string DefaultLocalApi = "http://localhost:5090";



    public static string Resolve(IConfiguration configuration)

    {

        var value = configuration[ConfigKey];

        if (!string.IsNullOrWhiteSpace(value))

            return value.Trim().TrimEnd('/');



        return DefaultLocalApi;

    }

}


