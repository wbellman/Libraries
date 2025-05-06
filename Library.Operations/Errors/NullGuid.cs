namespace Library.Operations.Errors;

public class NullGuid(
    Severity severity = Severity.Critical,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = 0
) : Error(
    "The passed id was null",
    severity, caller, filename, lineNumber
);