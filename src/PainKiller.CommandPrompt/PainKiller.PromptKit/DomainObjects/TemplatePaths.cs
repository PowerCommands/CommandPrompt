using PainKiller.PromptKit.Bootstrap;

namespace PainKiller.PromptKit.DomainObjects;
using System.IO;

public class TemplatePaths
{
    public TemplatePaths(string sourceModulePath, string outputPath, string projectName, string configurationTemplate)
    {
        var coreLibRoot = Directory.GetParent(sourceModulePath)!.FullName;
        var root = Directory.GetParent(coreLibRoot)!.FullName;
        var promptKitRoot = Path.Combine(root, $"{nameof(PainKiller)}.{nameof(PromptKit)}");
        var targetApplicationRoot = Path.Combine(outputPath, projectName, $"{projectName}Client");
        var readLineRoot = Path.Combine(root, $"{nameof(PainKiller)}.{nameof(ReadLine)}");
        CoreLibRoot = new TemplatePath(Source: coreLibRoot, Target: Path.Combine(outputPath, projectName,$"{nameof(PainKiller)}.{nameof(CommandPrompt)}.CoreLib"));
        ApplicationRoot = new TemplatePath(Source: promptKitRoot, Target: targetApplicationRoot);
        ModulesRoot = new TemplatePath(Source: sourceModulePath, Target: Path.Combine(outputPath, $"{nameof(PainKiller)}.{nameof(CommandPrompt)}.CoreLib", "Modules"));
        ConfigurationYamlPath = new TemplatePath(Source: Path.Combine(promptKitRoot,"Templates", configurationTemplate), Target: Path.Combine(targetApplicationRoot, $"{nameof(CommandPromptConfiguration)}.yaml"));
        ModulesConfigurationPath = new TemplatePath(Source: Path.Combine(coreLibRoot, "Configuration", "DomainObjects", "ModulesConfiguration.cs"), Target: Path.Combine(outputPath, projectName, "PainKiller.CommandPrompt.CoreLib", "Configuration", "DomainObjects", "ModulesConfiguration.cs"));
        Root = new TemplatePath(root, outputPath);
        ReadLineRoot = new TemplatePath(readLineRoot, Path.Combine(outputPath, projectName ,$"{nameof(PainKiller)}.{nameof(ReadLine)}"));
    }
    public TemplatePath Root { get; init; }
    public TemplatePath ReadLineRoot { get; init; }
    public TemplatePath CoreLibRoot { get; init; }
    public  TemplatePath ApplicationRoot { get; init; }
    public TemplatePath ModulesRoot { get; init; }
    public TemplatePath ConfigurationYamlPath { get; init; }
    public TemplatePath ModulesConfigurationPath { get; init; }
}