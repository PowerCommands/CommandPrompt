using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Core.Utils;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using Spectre.Console;

namespace PainKiller.PromptKit.Managers;

public class TemplateManager(string projectName, string modulesDirectory, string outputDirectory)
{
    private readonly ILogger<TemplateManager> _logger = LoggerProvider.CreateLogger<TemplateManager>();
    public void Run()
    {
        var modules = DiscoverModules();
        _logger.LogDebug($"Modules found: {string.Join(',', modules)}");
        DisplayModuleSelection(modules);
        var copyManager = new CopyManager();
        var projectOutputDirectory = Path.Combine(outputDirectory, projectName);
        if(Directory.Exists(projectOutputDirectory)) Directory.Delete(projectOutputDirectory, recursive: true);
        Directory.CreateDirectory(projectOutputDirectory);
        copyManager.CopyCoreProject(modulesDirectory, projectOutputDirectory, modules.Select(m => m.Name).ToList());
    }
    private List<(string Name, string Description)> DiscoverModules()
    {
        var modules = new List<(string Name, string Description)>();
        foreach (var dir in Directory.GetDirectories(modulesDirectory))
        {
            var moduleName = Path.GetFileName(dir);
            var readmePath = Path.Combine(dir, "readme.md");
            if (File.Exists(readmePath))
            {
                var lines = File.ReadLines(readmePath).Take(2).ToList();
                if (lines.Count > 0)
                {
                    var rawDescription = lines.Last() ?? "No description available";
                    var description = MarkdownToSpectreConverter.Convert(rawDescription);
                    modules.Add((moduleName, description));
                }
            }
        }
        return modules;
    }
    private void DisplayModuleSelection(List<(string Name, string Description)> modules)
    {
        var choices = modules.Select(m => $"{m.Name} - {m.Description}").ToList();
        var selectedModules = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select the modules to include:")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle a module, [green]<enter>[/] to accept)[/]")
                .AddChoices(choices)
                .NotRequired());

        ConsoleService.Writer.WriteDescription("Selected modules", $"{string.Join(", ", selectedModules.Select(s => s.Split(' ').First()))}");
    }
}
    