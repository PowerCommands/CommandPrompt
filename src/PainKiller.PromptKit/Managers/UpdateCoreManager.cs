using System.Security.Cryptography;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.PromptKit.Extensions;
using Spectre.Console;

namespace PainKiller.PromptKit.Managers;

public class UpdateCoreManager(string targetRoot, IConsoleWriter writer)
{
    private readonly string _sourceCoreLibPath = Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).GetRoot().FullName, "PainKiller.CommandPrompt.CoreLib", "Core");
    public void Update()
    {
        var filesToUpdate = new List<(string source, string target)>();
        var sourceFiles = Directory.GetFiles(_sourceCoreLibPath, "*.cs", SearchOption.AllDirectories).Where(f => !f.Contains("\\Commands\\") && !f.Contains("/Commands/")).ToList();

        foreach (var sourceFile in sourceFiles)
        {
            var relativePath = Path.GetRelativePath(_sourceCoreLibPath, sourceFile);
            var targetFile = Path.Combine(targetRoot, "PainKiller.CommandPrompt.CoreLib", "Core", relativePath);

            if (!File.Exists(targetFile))
            {
                filesToUpdate.Add((sourceFile, targetFile));
                continue;
            }

            var sourceChecksum = FileChecksum(sourceFile);
            var targetChecksum = FileChecksum(targetFile);

            if (sourceChecksum != targetChecksum)
            {
                filesToUpdate.Add((sourceFile, targetFile));
            }
        }

        if (!filesToUpdate.Any())
        {
            writer.WriteLine("No updates available.");
            return;
        }

        writer.WriteHeadLine("The following files are new or changed:");
        foreach (var file in filesToUpdate)
        {
            writer.WriteLine(Path.GetRelativePath(targetRoot, file.target));
        }

        var confirm = AnsiConsole.Confirm("Do you want to update these files?");
        if (!confirm)
        {
            writer.WriteLine("Update canceled.");
            return;
        }

        foreach (var (source, target) in filesToUpdate)
        {
            var targetDirectory = Path.GetDirectoryName(target);
            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory!);
            File.Copy(source, target, true);
            writer.WriteSuccessLine($"✅ {Path.GetRelativePath(targetRoot, target)} updated.");
        }

        writer.WriteSuccessLine("✅ All selected files have been updated.");
    }
    private static string FileChecksum(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }
}