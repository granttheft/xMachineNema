using XMachine.Connectors.Contracts;

namespace XMachine.Connectors.Mapping;

/// <summary>Minimal raw → canonical evaluation (no scripting / expressions).</summary>
public sealed class MappingEvaluationPipeline
{
    public IReadOnlyDictionary<string, CanonicalValue> EvaluatePlaceholder(
        MappingEvaluationContext context,
        IReadOnlyList<MappingFieldPath> rules)
    {
        var output = new Dictionary<string, CanonicalValue>(StringComparer.OrdinalIgnoreCase);

        foreach (var rule in rules)
        {
            if (!context.RawValues.TryGetValue(rule.RawField, out var raw))
                continue;

            var transformed = SimpleMappingTransform.TryApply(null, raw);
            if (transformed is not null)
                output[rule.CanonicalField] = transformed;
        }

        return output;
    }
}
