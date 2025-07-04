using System.Text.Json;

namespace Libraries.Json;

public static class SerializationFunctions
{
    public static JsonSerializerOptions DefaultOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static Outcome<T> FromJson<T>(
        string json,
        JsonSerializerOptions? options = null
    )
    {
        try
        {
            var obj = JsonSerializer.Deserialize<T>(
                json, options ?? DefaultOptions
            );

            if (obj is null)
                return Failure<T>(
                    new CouldNotParseJson<T>(json)
                );

            return Outcome<T>.Success(obj);
        }
        catch (Exception ex)
        {
            return Failure<T>(
                new CouldNotParseJson<T>(json, ex)
            );
        }
    }

    public static string ToJson<T>(
        T obj,
        JsonSerializerOptions? options = null
    ) => JsonSerializer.Serialize(
        obj, options ?? DefaultOptions
    );
}