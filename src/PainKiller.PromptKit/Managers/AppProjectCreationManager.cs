using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.PromptKit.Bootstrap;
using PainKiller.PromptKit.Commands;
using PainKiller.PromptKit.Configuration;
using PainKiller.PromptKit.DomainObjects;

namespace PainKiller.PromptKit.Managers;

public class AppProjectCreationManager(TemplatePaths paths, string projectName)
{
    private readonly ILogger<AppProjectCreationManager> _logger = LoggerProvider.CreateLogger<AppProjectCreationManager>();
    public void CreateAppProject(List<string> ignores)
    {
        CreateRoot();
        CreateBootstrap();
        CreateConfiguration();
        CreateCommands();
    }
    public void CreateSolutionFile()
    {
        var sourceSlnFilePath = Path.Combine(paths.Root.Source, $"{nameof(PainKiller)}.{nameof(CommandPrompt)}.sln");
        var content = File.ReadAllText(sourceSlnFilePath);
        content = content.Replace($"{nameof(PainKiller)}.{nameof(PromptKit)}", $"{projectName}Client");
        var targetSlnFilePath = Path.Combine(Directory.GetParent(paths.ApplicationRoot.Target)!.FullName, $"{projectName}.sln");
        File.WriteAllText(targetSlnFilePath, content);
        _logger.LogDebug($"Create {targetSlnFilePath}");
    }
    private string ReadFileAndReplace(string fileName, string find, string replace)
    {
        var content = File.ReadAllText(fileName);
        var retVal = content.Replace(find, replace);
        return retVal;
    }
    private void CreateRoot()
    {
        _logger.LogDebug($"Copy files from root directory {paths.ApplicationRoot}");
        Environment.CurrentDirectory = paths.ApplicationRoot.Source;
        
        var programContent = ReadFileAndReplace($"{nameof(Program)}.cs", "PainKiller.PromptKit", $"{projectName}Client");
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, $"{nameof(Program)}.cs"), programContent);

        File.Copy("PainKiller.PromptKit.csproj", Path.Combine(paths.ApplicationRoot.Target, $"{projectName}Client.csproj"));
        _logger.LogDebug($"Copy PainKiller.PromptKit.csproj to {projectName}.csproj");
    }
    private void CreateBootstrap()
    {
        Directory.CreateDirectory(Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap)));
        _logger.LogDebug($"Create directory {Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap))}");
        
        var startupContent = ReadFileAndReplace(Path.Combine($"{nameof(Bootstrap)}", $"{nameof(Startup)}.cs"), "PainKiller.PromptKit", $"{projectName}Client");
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap), $"{nameof(Startup)}.cs"), startupContent);
    }
    private void CreateConfiguration()
    {
        Directory.CreateDirectory(Path.Combine(paths.ApplicationRoot.Target, nameof(Configuration)));
        _logger.LogDebug($"Create directory {Path.Combine(paths.ApplicationRoot.Target, nameof(Configuration))}");

        var commandPromptConfigurationContent = @"using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
namespace PainKiller.PromptKit.Configuration;
public class CommandPromptConfiguration : ApplicationConfiguration
{
    public CustomConfiguration Custom { get; set; } = new();
}".Replace("PainKiller.PromptKit", $"{projectName}Client");
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Configuration), $"{nameof(CommandPromptConfiguration)}.cs"), commandPromptConfigurationContent);
        _logger.LogDebug($"Create {Path.Combine(paths.ApplicationRoot.Target, nameof(Configuration), $"{nameof(CommandPromptConfiguration)}.cs")}");
        var customConfigurationContent = @"namespace PainKiller.PromptKit.Configuration;
public class CustomConfiguration
{
    //Put your custom configuration here, feel free to rename this, don´t forget to update the yaml configuration file.
}".Replace("PainKiller.PromptKit", $"{projectName}Client");
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Configuration), "CustomConfiguration.cs"), customConfigurationContent);
        _logger.LogDebug($"Create {Path.Combine(paths.ApplicationRoot.Target, nameof(Configuration), "CustomConfiguration.cs")}");
    }
    private void CreateCommands()
    {
        Directory.CreateDirectory(Path.Combine(paths.ApplicationRoot.Target, nameof(Commands)));

        var demoCommandContent = ReadFileAndReplace(Path.Combine(nameof(Commands), $"{nameof(DemoCommand)}.cs"), "PainKiller.PromptKit", $"{projectName}Client");
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Commands),$"{nameof(DemoCommand)}.cs"), demoCommandContent);
    }
}