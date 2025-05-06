namespace Library.Operations.Errors;

public class EmptyGuid(
    Severity severity = Severity.Critical,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = 0
) : Error(
    "The passed id was empty",
    severity, caller, filename, lineNumber
);