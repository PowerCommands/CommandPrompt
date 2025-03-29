using PainKiller.CommandPrompt.CoreLib.Core.Utils;
using Spectre.Console;

namespace PainKiller.PromptKit.Managers;

public class PublisherManager(string modulesDirectory)
{
    public void Run()
    {
        var modules = DiscoverModules();
        DisplayModuleSelection(modules);
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
        AnsiConsole.MarkupLine($"[green]Selected modules:[/] {string.Join(", ", selectedModules)}");
    }
}
    