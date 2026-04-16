namespace XMachine.Connectors.Mapping;

/// <summary>Input bag for mapping evaluation (typically raw tag reads).</summary>
public sealed class MappingEvaluationContext
{
    public MappingEvaluationContext(IReadOnlyDictionary<string, string> rawValues) =>
        RawValues = rawValues;

    public IReadOnlyDictionary<string, string> RawValues { get; }
}
