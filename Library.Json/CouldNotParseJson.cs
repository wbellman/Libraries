using System.Runtime.CompilerServices;
using Library.Operations.Errors;
using Library.Operations.Indicators;

namespace Libraries.Json;

public class CouldNotParseJson<T>(
    string json,
    Exception? exception = null,
    Severity severity = Severity.Warning,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    BuildMessage(json, exception),
    severity, caller, filename, lineNumber
)
{
    private static string BuildMessage(string json, Exception? exception)
        => exception is null
            ? $"Could not deserialize: {typeof(T).Name} from JSON: {Write(json)}"
            : $"Could not deserialize: {typeof(T).Name} from JSON: {Write(json)} with exception: {exception.Message}";
    
    private static string Write(string json)
        => $"{NewLine}---{NewLine}{json}{NewLine}---{NewLine}";
}