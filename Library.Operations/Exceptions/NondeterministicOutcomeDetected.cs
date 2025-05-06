using System.Runtime.CompilerServices;

namespace Library.Operations.Exceptions;

public class NondeterministicOutcomeDetected(
    [CallerMemberName] string caller = "UNKNOWN CALLER"
) : Exception($"A nondeterministic outcome has been detected in {caller}");