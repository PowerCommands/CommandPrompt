namespace PainKiller.PromptKit.Bootstrap;
public class PromptKitConfiguration
{
    public string OutputPath { get; set; } = "output";
    public List<string> Ignores { get; set; } = [];
    public string ConfigurationTemplate { get; set; } = "default_CommandPromptConfiguration.yaml";
}