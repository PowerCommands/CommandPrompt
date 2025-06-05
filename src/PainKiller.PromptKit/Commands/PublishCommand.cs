using System.IO.Compression;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Configuration;
using PainKiller.PromptKit.Extensions;

namespace PainKiller.PromptKit.Commands;

[CommandDesign(description:  "Publish the current modules as an zipfile.", 
                  examples: ["//Publish current modules to zipfile.","publish"])]
public class PublishCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        try
        {
            var root = new DirectoryInfo(AppContext.BaseDirectory).GetRoot().FullName;
            var zipFilePath = Path.Combine(AppContext.BaseDirectory, $"{Configuration.PromptKit.UpdateFilename}.zip");
            var modulesDirectory = Path.Combine(root, "PainKiller.CommandPrompt.CoreLib", "Modules");

            if (!Directory.Exists(modulesDirectory))
            {
                ConsoleService.Writer.WriteError($"Modules directory not found: {modulesDirectory}", nameof(PublishCommand));
                return Nok();
            }
            if (File.Exists(zipFilePath))
            {
                ConsoleService.Writer.WriteWarning("Previous package found. Deleting...", nameof(PublishCommand));
                File.Delete(zipFilePath);
            }

            ConsoleService.Writer.WriteLine($"Packing modules from {modulesDirectory}...");

            var tempPath = Path.Combine(Path.GetTempPath(), $"{nameof(PromptKit)}_Modules");
            var tempModulesPath = Path.Combine(tempPath, "Modules");
            try
            {
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);

                Directory.CreateDirectory(tempModulesPath);
                ConsoleService.Writer.WriteLine($"Temporary directory created: {tempModulesPath}");
                
                foreach (var dir in Directory.GetDirectories(modulesDirectory))
                {
                    var moduleName = Path.GetFileName(dir);
                    var destinationDir = Path.Combine(tempModulesPath, moduleName);
                    Directory.CreateDirectory(destinationDir);
                    foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(dir, file);
                        var targetPath = Path.Combine(destinationDir, relativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                        File.Copy(file, targetPath);
                    }
                }
                ZipFile.CreateFromDirectory(tempPath, zipFilePath);
                ConsoleService.Writer.WriteSuccessLine($"✅ Modules have been published to {zipFilePath}");
            }
            finally
            {
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                    ConsoleService.Writer.WriteLine("Temporary directory deleted.");
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            ConsoleService.Writer.WriteError($"Failed to publish modules: {ex.Message}", nameof(PublishCommand));
            return Nok();
        }
    }
}