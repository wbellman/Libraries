using Microsoft.Extensions.Configuration;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: dotnet run <basePath> <jsonPath>");
    return;
}

var parameterBase = args[0].TrimEnd('/');
var jsonPath = args[1];

var config = new ConfigurationBuilder()
    .AddJsonFile(jsonPath, optional: false, reloadOnChange: false)
    .Build();

using var ssm = new AmazonSimpleSystemsManagementClient();

await PublishSection(
    ssm,
    config.GetChildren(),
    parameterBase
);

return;


static async Task PublishSection(
    IAmazonSimpleSystemsManagement ssm,
    IEnumerable<IConfigurationSection> sections,
    string path
)
{
    foreach (var section in sections)
    {
        var currentPath = $"{path}/{section.Key}";
        var children = section.GetChildren().ToArray();

        if (children.Length > 0)
        {
            await PublishSection(ssm, children, currentPath);
            continue;
        }

        var type = IsSecret(section.Key)
            ? ParameterType.SecureString
            : ParameterType.String;

        var value = section.Value ?? "";
        
        Console.WriteLine($"Publishing {currentPath} = {value}");
        await ssm.PutParameterAsync(new PutParameterRequest
        {
            Name = currentPath,
            Value = value,
            Type = type,
            Overwrite = true
        });
    }
}

static bool IsSecret(string key)
    => key.EndsWith("password", StringComparison.OrdinalIgnoreCase)
        || key.EndsWith("token", StringComparison.OrdinalIgnoreCase)
        || key.EndsWith("secret", StringComparison.OrdinalIgnoreCase)
        || key.EndsWith("key", StringComparison.OrdinalIgnoreCase)
        || key.EndsWith("DB", StringComparison.OrdinalIgnoreCase)
        ;