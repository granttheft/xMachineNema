namespace XMachine.Connectors.Mapping;

/// <summary>Raw → optional logical → canonical path for a single logical mapping step.</summary>
public sealed record MappingFieldPath(string RawField, string? LogicalField, string CanonicalField);
