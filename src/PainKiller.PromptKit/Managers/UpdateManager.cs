using System.IO.Compression;
using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;

namespace PainKiller.PromptKit.Managers;

public class UpdateManager(string updateFileName, string outputDirectory, IConsoleWriter writer)
{
    private readonly ILogger<UpdateManager> _logger = LoggerProvider.CreateLogger<UpdateManager>();
    public void Update()
    {
        var zipFileName = $"{updateFileName}.zip";
        var zipFilePath = Path.Combine(AppContext.BaseDirectory, zipFileName);

        if (!File.Exists(zipFilePath))
        {
            writer.WriteError($"ZIP-file '{zipFileName}' was not found in {AppContext.BaseDirectory}.");
            return;
        }
        var tempPath = CreateTemporaryDirectory();
        try
        {
            ExtractZipFile(zipFilePath, tempPath);

            var modulesDirectorySource = Path.Combine(tempPath, "Modules");

            var moduleManager = new ModuleManager(modulesDirectorySource, writer);
            var modules = moduleManager.ModulesDiscovery();
            _logger.LogDebug($"Modules found in update package: {string.Join(',', modules.Select(m => m.Name))}");

            if (!modules.Any())
            {
                writer.WriteError("No modules found in zip-file.");
                return;
            }
            var selectedModules = moduleManager.DisplayModuleSelection(modules);
            if (!selectedModules.Any())
            {
                writer.WriteLine("No module selected.");
                return;
            }
            var outputModulesPath = Path.Combine(outputDirectory, "PainKiller.CommandPrompt.CoreLib", "Modules");
            if (!Directory.Exists(outputModulesPath))
            {
                writer.WriteError($"Output directory '{outputModulesPath}' does not exist.");
                return;
            }
            foreach (var module in selectedModules)
            {
                var modulePath = Path.Combine(tempPath, "Modules",module);
                var destinationPath = Path.Combine(outputModulesPath, module);

                if (Directory.Exists(destinationPath))
                {
                    writer.WriteWarning($"Module {module} already exist and will be replaced with new version.");
                    Directory.Delete(destinationPath, true);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                Directory.Move(modulePath, destinationPath);
                writer.WriteSuccessLine($"✅ Module {module} has been updated.");
            }
            writer.WriteSuccessLine("All modules has been updated.");
        }
        catch (Exception ex)
        {
            writer.WriteError($"Error in update process: {ex.Message}");
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
                writer.WriteLine("Temporary directory deleted.");
            }
        }
    }
    private string CreateTemporaryDirectory()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{nameof(PromptKit)}_UpdateModules");
        if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
        Directory.CreateDirectory(tempPath);
        return tempPath;
    }
    private void ExtractZipFile(string zipFilePath, string tempPath)
    {
        try
        {
            ZipFile.ExtractToDirectory(zipFilePath, tempPath);
            writer.WriteLine($"Unpacked {zipFilePath} to {tempPath}");
        }
        catch (Exception ex)
        {
            writer.WriteError($"Error unpacking zip file: {ex.Message}");
            throw;
        }
    }
}