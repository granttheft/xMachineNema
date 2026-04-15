namespace XMachine.SharedKernel.Entities;

public interface IHasExternalReference
{
    string? SourceSystem { get; set; }
    string? SourceReference { get; set; }
}

