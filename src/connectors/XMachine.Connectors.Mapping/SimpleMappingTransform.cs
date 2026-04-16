using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Mapping;

/// <summary>Placeholder transform hook; replace with expression engine later.</summary>
public static class SimpleMappingTransform
{
    public static CanonicalValue? TryApply(string? transformKind, string rawValue)
    {
        if (string.IsNullOrWhiteSpace(transformKind))
            return new CanonicalStringValue(rawValue);

        return transformKind.ToLowerInvariant() switch
        {
            "trim" => new CanonicalStringValue(rawValue.Trim()),
            "upper" => new CanonicalStringValue(rawValue.ToUpperInvariant()),
            "lower" => new CanonicalStringValue(rawValue.ToLowerInvariant()),
            _ => new CanonicalStringValue(rawValue),
        };
    }
}
