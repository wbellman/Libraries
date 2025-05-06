using System.Runtime.CompilerServices;
using Library.Operations.Errors;
using Library.Operations.Indicators;
using Library.Operations.Outcomes;

namespace Library.Operations.Functions;

public static class Outcomes
{
    public static Outcome<TSuccess> Success<TSuccess>(TSuccess success)
        => Outcome<TSuccess>.Success(success);

    public static Outcome<TSuccess> Failure<TSuccess>(Error failure)
        => Outcome<TSuccess>.Failure<TSuccess>(failure);

    public static Outcome<TSuccess> IsNotNull<TSuccess>(TSuccess? item,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    ) where TSuccess : class
        => item is not null
            ? Success(item)
            : Failure<TSuccess>(new IsNull<TSuccess>(
                null, severity, caller, filename, lineNumber
            ));

    public static Outcome<T> StubAs<T>()
        => Failure<T>(new StubMethod());
}