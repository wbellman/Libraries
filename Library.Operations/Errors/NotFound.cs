namespace Library.Operations.Errors;

public class NotFound(
    string message,
    Severity severity = Severity.Warning,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    $"Not Found: {message}",
    severity, caller, filename, lineNumber
);