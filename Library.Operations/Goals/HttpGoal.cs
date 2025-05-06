using System.Net;

namespace Library.Operations.Goals;

/// <summary>
/// Represents a successful HTTP operation with a specific status code.
/// </summary>
/// <param name="statusCode">The HTTP status code indicating the result of the operation.</param>
public class HttpGoal(
    HttpStatusCode statusCode = HttpStatusCode.OK
) : Goal.Completed
{
    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode => statusCode;

    /// <summary>
    /// Returns the name of the class along with the status code.
    /// </summary>
    public override string ToString() => $"{GetType().Name}({StatusCode})";

    /// <summary>
    /// Represents an HTTP 201 Created status.
    /// </summary>
    public class Created() : HttpGoal(HttpStatusCode.Created);

    /// <summary>
    /// Represents an HTTP 202 Accepted status.
    /// </summary>
    public class Accepted() : HttpGoal(HttpStatusCode.Accepted);

    /// <summary>
    /// Represents an HTTP 204 No Content status.
    /// </summary>
    public class NoContent() : HttpGoal(HttpStatusCode.NoContent);
    
    public static Created WasCreated() => new Created();
    public static Accepted WasAccepted() => new Accepted();
    public static NoContent ReturnsNoContent() => new NoContent();
}