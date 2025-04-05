namespace PainKiller.PromptKit.Configuration;
public class PromptKitConfiguration
{
    public string OutputPath { get; set; } = "";
    public List<string> Ignores { get; set; } = [];
    public string ConfigurationTemplate { get; set; } = "default_CommandPromptConfiguration.yaml";
    public string UpdateFilename { get; set; } = "CommandPromptModules";
}