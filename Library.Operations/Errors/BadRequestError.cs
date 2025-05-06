namespace Library.Operations.Errors;

public class BadRequestError(
    string message,
    Severity severity = Severity.Warning,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    message, severity, caller, filename, lineNumber
);