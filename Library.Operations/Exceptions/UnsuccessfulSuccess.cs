using System.Runtime.CompilerServices;

namespace Library.Operations.Exceptions;

public class UnsuccessfulSuccess(
    [CallerMemberName] string caller = "UNKNOWN CALLER"
) : Exception($"A success was returned but it's output value was null in {caller}");