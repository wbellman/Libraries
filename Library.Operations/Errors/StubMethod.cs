namespace Library.Operations.Errors;

public class StubMethod(
    Severity severity = Severity.Warning,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    $"This is just a stub: {caller}",
    severity, caller, filename, lineNumber
);