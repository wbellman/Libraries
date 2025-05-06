namespace Library.Operations.Goals;

public interface IOperationGoal
{
}

/// <summary>
/// Represents the intent for a successful operation outcome. This abstraction serves three main purposes:
/// 1. Provide a uniform interpretation for various protocol interactions (REST, gRPC, Service Bus, etc.).
/// 2. Standardize API calls to follow a consistent return pattern.
/// 3. Create a more expressive API that is easily understandable by consumers.
/// </summary>
public class Goal : IOperationGoal
{
    /// <summary>
    /// Represents a successful operation that has completed.
    /// </summary>
    public class Completed : Goal
    {
    }
    
    /// <summary>
    /// Returns the name of the class.
    /// </summary>
    public override string ToString() => GetType().Name;

    /// <summary>
    /// Factory method to create a Completed goal.
    /// </summary>
    public static Completed WasCompleted() => new();

    /// <summary>
    /// Generic factory method to create a goal of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of goal to create.</typeparam>
    /// <returns>A new instance of the specified goal type.</returns>
    public static T Create<T>()
        where T : Goal, new()
        => new();
}