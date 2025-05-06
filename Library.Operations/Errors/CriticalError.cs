using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Library.Operations.Errors.Models;

namespace Library.Operations.Errors;

public class CriticalError(
    string message,
    Exception? caughtException = null,
    Severity severity = Severity.Critical,
    [CallerMemberName] string? caller = null,
    [CallerFilePath] string? filename = null,
    [CallerLineNumber] int lineNumber = -1
) : Error(
    message, severity, caller, filename, lineNumber
)
{
    public Exception? CaughtException { get; set; } = caughtException;

    [MemberNotNullWhen(true, nameof(CaughtException))]
    public bool Excepted => CaughtException is not null;

    public override string ToString()
    {
        var toString = base.ToString();

        if (!Excepted)
        {
            return toString;
        }

        var method = CaughtException.TargetSite?.Name;

        var frame = new StackTrace(
            CaughtException,
            true
        ).GetFrame(0);

        var file = "UNKNOWN FILE";
        var line = -1;

        if (frame != null)
        {
            file = frame.GetFileName();
            line = frame.GetFileLineNumber();
        }

        var callsite = new Callsite(method, file, line);

        toString += $"{toString} (EXCEPTED) \n" +
                    $" EXCEPTION => {CaughtException.GetType().Name} :: {CaughtException} -> {callsite}";
        ;

        return toString;
    }
}