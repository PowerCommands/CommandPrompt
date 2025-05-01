using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
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
}