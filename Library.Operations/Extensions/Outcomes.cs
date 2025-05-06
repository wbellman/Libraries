using System.Runtime.CompilerServices;
using Library.Operations.Errors;
using Library.Operations.Exceptions;
using Library.Operations.Indicators;
using Library.Operations.Outcomes;
using static Library.Operations.Functions.Outcomes;

namespace Library.Operations.Extensions;

// Convenience class to simplify syntax
public static class Outcome
{
    public static Outcome<T> OutcomeIsNotNull<T>(
        this Outcome<T?> value,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    )
        where T : class
        => value.Next(
            v => v is not null
                ? Success(v)
                : Failure<T>(new IsNull<T>(null, severity, caller, filename, lineNumber))
        );

    public static Task<Outcome<T>> OutcomeIsNotNull<T>(
        this Task<Outcome<T?>> value,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    ) where T : class
        => value.Next(
            v => v is not null
                ? Success(v)
                : Failure<T>(new IsNull<T>(null, severity, caller, filename, lineNumber))
        );

    public static Task<Outcome<TResult>> Next<TSource, TResult>(
        this Task<Outcome<TSource>> task,
        Func<TSource, Outcome<TResult>> nextFunc,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    ) => task.Next<TSource, TResult>(src => Task.FromResult(nextFunc(src)));

    public static async Task<Outcome<TResult>> Next<TSource, TResult>(
        this Task<Outcome<TSource>> task,
        Func<TSource, Task<Outcome<TResult>>> nextFunc,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    )
    {
        try
        {
            var outcome = await task;
            return outcome switch
            {
                FailureOutcome<TSource> asyncOutcome => Failure<TResult>(asyncOutcome.Value),
                SuccessOutcome<TSource> success =>
                    success.Value is null
                        ? throw new NondeterministicOutcomeDetected()
                        : await nextFunc(success.Value),
                _ => throw new NondeterministicOutcomeDetected()
            };
        }
        catch (Exception e)
        {
            return Failure<TResult>(new CriticalError($"EXCEPTED: {e.Message}", e));
        }
    }

    public static async Task<Outcome<TResult?>> WithOutcome<TResult>(
        this Task<TResult?> task,
        Func<Exception, Outcome<TResult?>>? onError = null,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    )
    {
        onError ??= e => Failure<TResult?>(new CriticalError($"EXCEPTED: {e.Message}", e));

        try
        {
            var result = await task;
            return Success(result);
        }
        catch (Exception e)
        {
            return onError(e);
        }
    }

    public static async Task<Outcome<TResult>> WithProtectedOutcome<TResult>(
        this Task task,
        TResult passedResult,
        Func<Exception, Outcome<TResult>>? onError = null,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    )
    {
        onError ??= e =>
            Failure<TResult>(new CriticalError($"EXCEPTED: {e.Message}", e, Severity.Critical, caller, filename,
                lineNumber));

        try
        {
            await task;
            return Success(passedResult);
        }
        catch (Exception e)
        {
            return onError(e);
        }
    }

    public static async Task<Outcome<List<TSuccess?>>> AggregateOutcomes<TSuccess>(
        this IEnumerable<Task<Outcome<TSuccess>>> tasks,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    )
    {
        var outcomes = await Task.WhenAll(tasks);

        var failures = outcomes
            .OfType<FailureOutcome<TSuccess>>()
            .ToList();

        if (failures.Count != 0)
        {
            return Failure<List<TSuccess?>>(new ComplexError<TSuccess>(failures));
        }

        var successes = outcomes
            .OfType<SuccessOutcome<TSuccess>>()
            .Select(o => o.Value)
            .ToList();

        return Success(successes);
    }

    public static Outcome<List<TSuccess?>> AggregateOutcomes<TSuccess>(
        this IEnumerable<Outcome<TSuccess?>> outcomes,
        Severity severity = Severity.Error,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? filename = null,
        [CallerLineNumber] int lineNumber = -1
    )
    {
        var all = outcomes.ToList();

        var failures = all
            .OfType<FailureOutcome<TSuccess?>>()
            .ToList();

        if (failures.Count != 0)
        {
            return Failure<List<TSuccess?>>(new ComplexError<TSuccess?>(
                failures, severity, caller, filename, lineNumber
            ));
        }

        var successes = all
            .OfType<SuccessOutcome<TSuccess?>>()
            .Select(o => o.Value)
            .ToList();

        return Success(successes);
    }
}