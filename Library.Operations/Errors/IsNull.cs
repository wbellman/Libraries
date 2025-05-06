namespace Library.Operations.Errors;

public abstract class IsNull(
    string message,
    Severity severity = Severity.Error,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    message, severity, caller, filename, lineNumber
);

public class IsNull<T>(
    string? additional = null,
    Severity severity = Severity.Error,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : IsNull(
    CreateMessage(caller, additional),
    severity, caller, filename, lineNumber
)
{
    private static string CreateMessage(string? caller = null, string? additional = null)
    {
        var error = $"Type {typeof(T).Name} was null in {caller ?? "UNKNOWN"}";
        return string.IsNullOrWhiteSpace(additional)
            ? error
            : $"{additional} => {error}";
    }
}