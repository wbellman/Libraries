using System.Collections.Generic;
using System.Text;
using Library.Operations.Outcomes;

namespace Library.Operations.Errors;

public class ComplexError<T>(
    List<FailureOutcome<T>> failures,
    Severity severity = Severity.Error,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    BuildComplexErrorMessage(failures),
    severity, caller, filename, lineNumber
)
{
    private static string BuildComplexErrorMessage(List<FailureOutcome<T>> failures)
    {
        var message = new StringBuilder();
        message.AppendLine($"Detected {failures.Count} failures:");
        foreach (var failure in failures)
        {
            message.AppendLine($" -> {failure.Value.Message}");
        }

        return message.ToString();
    }

    public List<FailureOutcome<T>> CollectedFailures { get; } = failures;
}