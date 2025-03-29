using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
namespace PainKiller.PromptKit.Bootstrap;
public class CommandPromptConfiguration : ApplicationConfiguration
{
    public PromptKitConfiguration PromptKit { get; set; } = new();
}