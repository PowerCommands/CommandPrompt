using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;

namespace PainKiller.PromptKit.Managers;

public class CopyManager
{
    private readonly ILogger<CopyManager> _logger = LoggerProvider.CreateLogger<CopyManager>();
    public void CopyCoreProject(string modulesDirectory, string outputDirectory, List<string> selectedModules)
    {
        var coreProjectDirectory = Directory.GetParent(modulesDirectory)?.FullName;

        _logger.LogDebug($"Copy files from root directory {coreProjectDirectory}");
        foreach (var file in Directory.GetFiles(coreProjectDirectory!))
        {
            var destFile = Path.Combine(outputDirectory, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
            _logger.LogDebug($"File {file} copied to {destFile}");
        }
        _logger.LogDebug($"Copy directories, ignore bin and obj directory");
        foreach (var directory in Directory.GetDirectories(coreProjectDirectory))
        {
            var dirName = Path.GetFileName(directory);
            if (dirName.Equals("bin", StringComparison.OrdinalIgnoreCase) || dirName.Equals("obj", StringComparison.OrdinalIgnoreCase)) continue;

            _logger.LogDebug("Handle Modules directory");
            if (dirName.Equals("Modules", StringComparison.OrdinalIgnoreCase))
            {
                var destModulesDir = Path.Combine(outputDirectory, "Modules");
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
                    }
                }
            }
            else
            {
                
                var destDir = Path.Combine(outputDirectory, dirName);
                IOService.CopyFolder(directory, destDir);
                _logger.LogDebug($"{directory} copied to {destDir}.");
            }
        }
    }
}
