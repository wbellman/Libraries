using Library.Operations.Errors.Models;

namespace Library.Operations.Errors;

public abstract class Error(
    string message,
    Severity severity,
    string? caller,
    string? filename,
    int lineNumber
)
{
    public string? Message { get; init; } = message;
    public Severity Severity { get; init; } = severity;
    public Callsite Callsite { get; init; } = new(caller, filename, lineNumber);

    public override string ToString()
        => $"{Severity}: {Message} at {Callsite}";
}