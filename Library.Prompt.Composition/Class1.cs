using Library.Operations.Goals;
using Library.Operations.Outcomes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using Scriban;

namespace Library.Prompt.Composition;

/*
 --- usage example

 var data = new Dictionary<string,string> {
    ["genre"] = "science fiction",
    ["tone"] = "dark and foreboding",
    ["setting"] = "unstable utopia"
 };

 var templates = Templates
  .StartWith("StoryBeatPrompt")
  .AddData(data);
  .Generate();

  --- yaml template example

     includes:
      - ../shared/guidelines.yml
      - ../shared/roles.yml

    template: |
      {{ roles.creative_writer }}

      # Task
      Generate the next story beat for a {{ genre }} story with {{ tone }} tone.

      ## Requirements
      - {{ guidelines.paragraph_count }}
      - Follow the user's instruction
      - Build on the current scene

      ## Input
      **User Instructions:** {{ instruction }}

      **Current Scene:** {{ scene }}

      ## Output
      Respond with valid JSON matching this structure:
      {{ output_example }}

    variables:
      output_type: "{{ output_type }}"

*/

public record CompositionOptions();

public class PromptComposer(
    IOptions<CompositionOptions> options,
    ILogger<PromptComposer> log
)
{
    private CompositionOptions Options => options.Value;
    private ILogger Log => log;

    public Outcome<List<string>> GetTemplateNames()
        => StubAs<List<string>>();

    public Outcome<Goal.Completed> TemplateExists(string templateName)
        => StubAs<Goal.Completed>();

    public Outcome<Goal.Completed> GetTemplate(
        string templateName,
        bool loadIncludes = true
    ) => StubAs<Goal.Completed>();

    public Outcome<Goal.Completed> Compose(
        string templateName,
        Dictionary<string, string> data
    ) => StubAs<Goal.Completed>();
}