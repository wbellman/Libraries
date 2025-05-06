namespace Library.Operations.Errors;

public class NoValidGuids(
    Severity severity = Severity.Critical,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = 0
) : Error(
    "All of the supplied ids were either null or empty.",
    severity, caller, filename, lineNumber
);