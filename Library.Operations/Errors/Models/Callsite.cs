namespace Library.Operations.Errors.Models;

public record Callsite(
    string? Caller,
    string? Filename,
    int LineNumber
)
{
    public override string ToString()
        => $"{Caller ?? "UNKNOWN CALLER"} in {Filename ?? "UNKNOWN FILE"}:{LineNumber}";
}