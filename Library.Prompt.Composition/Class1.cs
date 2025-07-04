using System.Reflection;
using Library.Prompt.Composition;
//using static NormalizationFunctions;

/*

public class EmbeddedPromptFactory : IPromptFactory
{
}


public class StoryService
{
    private readonly IPromptFactory _promptFactory;

    public async Task<T> GenerateStory<T>(
        IInstruction instruction,
        Dictionary<string, string> parameters)
        where T : IPromptObject<T>
    {
        var prompt = _promptFactory.CreatePrompt<T>("story_generation.yml",
            ("instruction", instruction.ToString()),
            ("scene", parameters["scene"]),
            ("genre", parameters["genre"]),
            ("tone", parameters["tone"])
        );

        return await _llmClient.GenerateAsync<T>(prompt);
    }

    public async Task<World.Character> CreateCharacter(
        string description,
        World.Story story)
    {
        var prompt = _promptFactory.CreatePrompt<World.Character>("character_creation.yml",
            ("description", description),
            ("story", story.ToString())
        );

        return await _llmClient.GenerateAsync<World.Character>(prompt);
    }
}


public interface IPromptFactory
{
    string CreatePrompt<TOutput>(string filename, params (string name, object data)[] parameters)
        where TOutput : IPromptObject<TOutput>;
}

public class PromptFactory : IPromptFactory
{
    private readonly Assembly _assembly;
    private readonly string _promptBasePath;
    private readonly string _modelName;
    private readonly ITemplateEngine _templateEngine;
    private readonly Dictionary<string, object> _sharedIncludes;

    public PromptFactory(string promptBasePath, string modelName, ITemplateEngine templateEngine)
    {
        _promptBasePath = promptBasePath;
        _modelName = NormalizeModelName(modelName);
        _templateEngine = templateEngine;
        _sharedIncludes = LoadSharedIncludes();
        _assembly = Assembly.GetExecutingAssembly();
    }

    public string CreatePrompt<TOutput>(string filename, params (string name, object data)[] parameters)
        where TOutput : IPromptObject<TOutput>
    {
        var templatePath = Path.Combine(_promptBasePath, _modelName, filename);
        
        var resourceName = $"YourProject.Prompts.{_modelName}.{filename}";

        using var stream = _assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new FileNotFoundException($"Prompt template not found: {resourceName}");

        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        
        var templateDefinition = LoadTemplateWithIncludes(content);
        var context = BuildContext<TOutput>(templateDefinition, parameters);

        return _templateEngine.Process(templateDefinition.Template, context);
    }

    private Dictionary<string, object> BuildContext<TOutput>(
        TemplateDefinition definition,
        (string name, object data)[] parameters)
        where TOutput : IPromptObject<TOutput>
    {
        var context = new Dictionary<string, object>(_sharedIncludes);

        // Add user parameters
        foreach (var (name, data) in parameters)
        {
            context[name] = data;
        }

        // Add output type information
        context["output_type"] = typeof(TOutput).Name;
        context["output_example"] = TOutput.GetExample();

        // Add template variables
        foreach (var variable in definition.Variables)
        {
            context[variable.Key] = ProcessVariable(variable.Value, context);
        }

        return context;
    }

    private TemplateDefinition LoadTemplateWithIncludes(string templatePath)
    {
        var content = File.ReadAllText(templatePath);
        var definition = DeserializeYaml<TemplateDefinition>(content);

        // Process includes
        if (definition.Includes != null)
        {
            foreach (var include in definition.Includes)
            {
                var includePath = Path.Combine(Path.GetDirectoryName(templatePath), include);
                var includeContent = DeserializeYaml<Dictionary<string, object>>(File.ReadAllText(includePath));

                // Merge includes into shared context
                foreach (var kvp in includeContent)
                {
                    _sharedIncludes[kvp.Key] = kvp.Value;
                }
            }
        }

        return definition;
    }
    
    private Dictionary<string, object> LoadSharedIncludes()
    {
        var sharedResources = _assembly.GetManifestResourceNames()
            .Where(name => name.Contains(".Prompts.shared."))
            .ToArray();

        var includes = new Dictionary<string, object>();

        foreach (var resource in sharedResources)
        {
            using var stream = _assembly.GetManifestResourceStream(resource);
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            var data = DeserializeYaml<Dictionary<string, object>>(content);
            foreach (var kvp in data)
            {
                includes[kvp.Key] = kvp.Value;
            }
        }

        return includes;
    }
}


public static class NormalizationFunctions {
    public static string NormalizeModelName(string modelName)
    {
        // Convert "gpt-4" -> "openai", "claude-3" -> "anthropic", etc.
        return modelName.ToLower() switch
        {
            var name when name.StartsWith("gpt") => "openai",
            var name when name.StartsWith("claude") => "anthropic",
            var name when name.StartsWith("llama") => "ollama",
            _ => modelName.ToLower()
        };
    }
}

*/