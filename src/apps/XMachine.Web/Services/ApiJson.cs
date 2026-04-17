using System.Text.Json;
using System.Text.Json.Serialization;

namespace XMachine.Web.Services;

internal static class ApiJson
{
    internal static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };
}
