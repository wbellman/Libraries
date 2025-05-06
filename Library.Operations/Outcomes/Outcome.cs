using System.Diagnostics.CodeAnalysis;
using Library.Operations.Errors;

namespace Library.Operations.Outcomes;

public abstract class Outcome<TSuccess>
{
    public static Outcome<TSuccess> Success(
        TSuccess success
    ) => new SuccessOutcome<TSuccess>(success);

    public static Outcome<T> Failure<T>(
        Error failure
    ) => new FailureOutcome<T>(failure);

    public abstract Outcome<T> Next<T>(
        Func<TSuccess, Outcome<T>> func
    );

    public abstract Task<Outcome<T>> Next<T>(
        Func<TSuccess, Task<Outcome<T>>> func
    );

    public abstract bool Failed { get; }
    public bool Succeeded => !Failed;

    public abstract Error? GetError();
    public abstract bool Resolve([NotNullWhen(true)] out TSuccess? value);
    public abstract bool DidSucceed([NotNullWhen(true)] out TSuccess? value, [NotNullWhen(false)] out Error? error);
    public abstract bool DidNotSucceed([NotNullWhen(false)] out TSuccess? value, [NotNullWhen(true)] out Error? error);
    public abstract void Unpack(out TSuccess? value, out Error? error);
}

public class SuccessOutcome<TS>(TS? value) : Outcome<TS>
{
    public TS? Value { get; } = value;

    public override Outcome<T> Next<T>(
        Func<TS, Outcome<T>> func
    ) => Failed
        ? Failure<T>(new IsNull<TS>())
        : func(Value);

    public override async Task<Outcome<T>> Next<T>(
        Func<TS, Task<Outcome<T>>> func
    ) => Failed
        ? Failure<T>(new IsNull<TS>())
        : await func(Value);

    [MemberNotNullWhen(false, nameof(Value))]
    public override bool Failed => Value == null;

    public override Error? GetError() => null;

    public override bool Resolve([NotNullWhen(true)] out TS? value)
    {
        value = Value!;
        return true;
    }

    public override bool DidSucceed([NotNullWhen(true)] out TS? value, [NotNullWhen(false)] out Error? error)
    {
        value = Value;
        error = null;
        return Value != null;
    }

    public override bool DidNotSucceed([NotNullWhen(false)] out TS? value, [NotNullWhen(true)] out Error? error)
    {
        value = Value;
        error = null;
        return Value == null;
    }

    public override void Unpack(out TS? value, out Error? error)
    {
        value = Value;
        error = null;
    }
}

public class FailureOutcome<TS>(Error value) : Outcome<TS>
{
    public Error Value { get; } = value;

    public override Outcome<T> Next<T>(
        Func<TS, Outcome<T>> func
    ) => new FailureOutcome<T>(Value);

    public override Task<Outcome<T>> Next<T>(
        Func<TS, Task<Outcome<T>>> func
    ) => Task.FromResult(new FailureOutcome<T>(Value) as Outcome<T>);

    public override bool Failed => true;

    public override Error GetError() => Value;

    public override bool Resolve([NotNullWhen(true)] out TS? value)
    {
        value = default;
        return false;
    }

    public override bool DidSucceed([NotNullWhen(true)] out TS? value, [NotNullWhen(false)] out Error? error)
    {
        value = default;
        error = Value;
        return false;
    }

    public override bool DidNotSucceed([NotNullWhen(false)] out TS? value, [NotNullWhen(true)] out Error? error)
    {
        value = default;
        error = Value;
        return true;
    }

    public override void Unpack(out TS? value, out Error? error)
    {
        value = default;
        error = Value;
    }
}