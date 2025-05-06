namespace Library.Operations.Errors;

public class WasEmpty(
    string message,
    Severity severity = Severity.Warning,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    $"Was Empty: {message}",
    severity, caller, filename, lineNumber
);