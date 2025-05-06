using Library.Operations.Errors;
using Library.Operations.Extensions;
using Library.Operations.Outcomes;

namespace Library.Operations.Functions;

public static class GuidFunctions
{
    public static Outcome<Guid> GuidIsValid(Guid? id)
        => GuidIsNotNull(id)
            .Next(GuidIsNotEmpty);

    public static Outcome<Guid> GuidIsNotNull(Guid? id)
        => id is null
            ? Failure<Guid>(new NullGuid())
            : Success(id.Value);

    public static Outcome<Guid> GuidIsNotEmpty(Guid id)
        => id == Guid.Empty
            ? Failure<Guid>(new EmptyGuid())
            : Success(id);

    public static Outcome<List<Guid>> OnlyValidGuids(IEnumerable<Guid?> ids)
        => ids
            .Select(GuidIsValid)
            .AggregateOutcomes()
            .Next(gs
                => gs.Count == 0
                    ? Failure<List<Guid>>(new NoValidGuids())
                    : Success(gs)
            );
}