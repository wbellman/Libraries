using System.Text.Json;
using Libraries.Json;
using Library.Operations.Outcomes;

namespace Library.Prompt.Composition;

public static class JsonFunctions
{
    private static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static Outcome<T> FromJson<T>(string json)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<T>(json, Options);
            
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
    
    public static string ToJson<T>(T obj) => JsonSerializer.Serialize(obj, Options);
}