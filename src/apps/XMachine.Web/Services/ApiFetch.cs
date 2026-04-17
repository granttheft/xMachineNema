namespace XMachine.Web.Services;

public readonly record struct ApiFetch<T>(bool Loading, T? Data, string? Error)
{
    public static ApiFetch<T> InProgress() => new(true, default, null);
    public static ApiFetch<T> Ok(T data) => new(false, data, null);
    public static ApiFetch<T> Fail(string message) => new(false, default, message);
}
