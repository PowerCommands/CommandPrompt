using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
using PainKiller.PromptKit.Bootstrap;
using PainKiller.PromptKit.Commands;
using PainKiller.PromptKit.DomainObjects;

namespace PainKiller.PromptKit.Managers;
public class CopyManager(TemplatePaths paths)
{
    private readonly ILogger<CopyManager> _logger = LoggerProvider.CreateLogger<CopyManager>();
    public void CopyCoreProject(List<string> selectedModules, List<string> ignores)
    {
        Directory.CreateDirectory(paths.Root.Target);
        Directory.CreateDirectory(paths.CoreLibRoot.Target);
        _logger.LogDebug($"Directory {paths.Root.Target} created.");
        _logger.LogDebug($"Copy files from root directory {paths.CoreLibRoot.Source}");
        foreach (var file in Directory.GetFiles(paths.CoreLibRoot.Source))
        {
            if (ignores.Contains(Path.GetFileName(file), StringComparer.OrdinalIgnoreCase)) continue;
            var destFile = Path.Combine(paths.CoreLibRoot.Target, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
            _logger.LogDebug($"File {file} copied to {destFile}");
        }
        _logger.LogDebug($"Copy directories, ignore bin and obj directory");
        foreach (var directory in Directory.GetDirectories(paths.CoreLibRoot.Source))
        {
            var dirName = Path.GetFileName(directory);
            if (ignores.Contains(dirName, StringComparer.OrdinalIgnoreCase)) continue;
            _logger.LogDebug("Handle Modules directory");
            if (dirName.Equals("Modules", StringComparison.OrdinalIgnoreCase))
            {
                var destModulesDir = Path.Combine(paths.CoreLibRoot.Target, "Modules");
                Directory.CreateDirectory(destModulesDir);

                foreach (var moduleDir in Directory.GetDirectories(directory))
                {
                    var moduleName = Path.GetFileName(moduleDir);
                    if (selectedModules.Contains(moduleName))
                    {
                        _logger.LogDebug($"Modul [{moduleName}] was selected copy the directory.");
                        var destModuleDir = Path.Combine(destModulesDir, moduleName);
                        IOService.CopyFolder(moduleDir, destModuleDir);
                        _logger.LogDebug($"{moduleDir} copied to {destModuleDir}.");
                        ConsoleService.Writer.WriteSuccessLine($"✅ Copy module {moduleName}");
                    }
                }
            }
            else
            {
                var destDir = Path.Combine(paths.CoreLibRoot.Target, dirName);
                IOService.CopyFolder(directory, destDir);
                _logger.LogDebug($"{directory} copied to {destDir}.");
            }
        }
    }
    public void CreateAppProject(string projectName, List<string> ignores)
    {
        _logger.LogDebug($"Copy files from root directory {paths.ApplicationRoot}");
        Environment.CurrentDirectory = paths.ApplicationRoot.Source;

        //Program.cs
        var programContent = ReadFileAndReplace($"{nameof(Program)}.cs", "PainKiller.PromptKit", projectName);
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, $"{nameof(Program)}.cs"), programContent);

        File.Copy("PainKiller.PromptKit.csproj", Path.Combine(paths.ApplicationRoot.Target, $"{projectName}Client.csproj"));
        _logger.LogDebug($"Copy PainKiller.PromptKit.csproj to {projectName}.csproj");
        
        Directory.CreateDirectory(Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap)));
        _logger.LogDebug($"Create directory {Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap))}");
        var commandPromptConfigurationContent = @"using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
namespace PainKiller.PromptKit.Bootstrap;
public class CommandPromptConfiguration : ApplicationConfiguration
{
    public CustomConfiguration Custom { get; set; } = new();
}".Replace("PainKiller.PromptKit", projectName);
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap), $"{nameof(CommandPromptConfiguration)}.cs"), commandPromptConfigurationContent);
        _logger.LogDebug($"Create {Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap), $"{nameof(CommandPromptConfiguration)}.cs")}");
        var customConfigurationContent = @"namespace PainKiller.PromptKit.Bootstrap;
public class CustomConfiguration
{
    //Put your custom configuration here, feel free to rename this, don´t forget to update the yaml configuration file.
}".Replace("PainKiller.PromptKit", projectName);
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap), "CustomConfiguration.cs"), customConfigurationContent);
        _logger.LogDebug($"Create {Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap), "CustomConfiguration.cs")}");

        var startupContent = ReadFileAndReplace(Path.Combine($"{nameof(Bootstrap)}", $"{nameof(Startup)}.cs"), "PainKiller.PromptKit", projectName);
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Bootstrap), $"{nameof(Startup)}.cs"), startupContent);
        
        Directory.CreateDirectory(Path.Combine(paths.ApplicationRoot.Target, nameof(Commands)));

        var demoCommandContent = ReadFileAndReplace(Path.Combine(nameof(Commands), $"{nameof(DemoCommand)}.cs"), "PainKiller.PromptKit", projectName);
        File.WriteAllText(Path.Combine(paths.ApplicationRoot.Target, nameof(Commands),$"{nameof(DemoCommand)}.cs"), demoCommandContent);
    }
    public void CreateSolutionFile(string projectName)
    {
        var sourceSlnFilePath = Path.Combine(paths.Root.Source, $"{nameof(PainKiller)}.{nameof(CommandPrompt)}.sln");
        var content = File.ReadAllText(sourceSlnFilePath);
        content = content.Replace($"{nameof(PainKiller)}.{nameof(PromptKit)}", $"{projectName}Client");
        var targetSlnFilePath = Path.Combine(Directory.GetParent(paths.ApplicationRoot.Target)!.FullName, $"{projectName}.sln");
        File.WriteAllText(targetSlnFilePath, content);
        _logger.LogDebug($"Create {targetSlnFilePath}");
    }

    public string ReadFileAndReplace(string fileName, string find, string replace)
    {
        var content = File.ReadAllText(fileName);
        var retVal = content.Replace(find, replace);
        return retVal;
    }
}