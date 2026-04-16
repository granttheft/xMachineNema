namespace XMachine.Connectors.Contracts;

/// <summary>Minimal typed canonical field value (extend later).</summary>
public abstract record CanonicalValue;

public sealed record CanonicalStringValue(string Value) : CanonicalValue;

public sealed record CanonicalNumberValue(double Value) : CanonicalValue;

public sealed record CanonicalBooleanValue(bool Value) : CanonicalValue;

public sealed record CanonicalBinaryValue(byte[] Value) : CanonicalValue;
