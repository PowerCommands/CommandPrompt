namespace PainKiller.PromptKit.DomainObjects;
using System.IO;

public class TemplatePaths
{
    public TemplatePaths(string sourceModulePath, string outputPath, string projectName)
    {
        var coreLibRoot = Directory.GetParent(sourceModulePath)!.FullName;
        var solutionRoot = Directory.GetParent(coreLibRoot)!.FullName;
        var promptKitRoot = Path.Combine(solutionRoot, "PainKiller.PromptKit");
        CoreLibRoot = new TemplatePath(Source: coreLibRoot, Target: Path.Combine(outputPath, projectName,"PainKiller.CommandPrompt.CoreLib"));
        ApplicationRoot = new TemplatePath(Source: promptKitRoot, Target: Path.Combine(outputPath, projectName, projectName));
        ModulesRoot = new TemplatePath(Source: sourceModulePath, Target: Path.Combine(outputPath, "PainKiller.CommandPrompt.CoreLib", "Modules"));
        ConfigurationYamlPath = new TemplatePath(Source: Path.Combine(promptKitRoot, "CommandPromptConfiguration.yaml"), Target: Path.Combine(outputPath, projectName, "CommandPromptConfiguration.yaml"));
        ModulesConfigurationPath = new TemplatePath(Source: Path.Combine(coreLibRoot, "Configuration", "DomainObjects", "ModulesConfiguration.cs"), Target: Path.Combine(outputPath, "PainKiller.CommandPrompt.CoreLib", "Configuration", "DomainObjects", "ModulesConfiguration.cs"));
        SolutionRoot = new TemplatePath(solutionRoot, outputPath);
    }
    public TemplatePath SolutionRoot { get; init; }
    public TemplatePath CoreLibRoot { get; init; }
    public  TemplatePath ApplicationRoot { get; init; }
    public TemplatePath ModulesRoot { get; init; }
    public TemplatePath ConfigurationYamlPath { get; init; }
    public TemplatePath ModulesConfigurationPath { get; init; }
}