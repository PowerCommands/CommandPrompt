using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.Utils;
using Spectre.Console;

namespace PainKiller.PromptKit.Managers;

public class ModuleManager(string modulesDirectory, IConsoleWriter writer)
{
    public List<(string Name, string Description)> ModulesDiscovery()
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
                    var rawDescription = string.Join(' ', lines) ?? "No description available";
                    var description = MarkdownToSpectreConverter.Convert(rawDescription);
                    modules.Add((moduleName, description));
                }
            }
        }
        return modules;
    }
    public List<string> DisplayModuleSelection(List<(string Name, string Description)> modules)
    {
        while (true)
        {
            var choices = modules.Select(m => $"{m.Name} - {m.Description}").ToList();
            var selectedModules = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Select the modules to include:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                    .InstructionsText("[grey](Press [blue]<space>[/] to toggle a module, [magenta]<enter>[/] to accept)[/]")
                    .AddChoices(choices)
                    .NotRequired());

            if (!selectedModules.Any())
            {
                writer.WriteLine("No modules selected.");
                return new List<string>();
            }

            var selectedModuleNames = selectedModules.Select(s => s.Split(' ').First()).ToList();
            writer.WriteDescription("Selected modules", $"{string.Join(", ", selectedModuleNames)}");

            var confirmation = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Do you want to proceed with the selected modules?")
                    .AddChoices("Yes", "No, redo selection", "Cancel"));

            if (confirmation == "Yes")
            {
                return selectedModuleNames;
            }
            if (confirmation == "Cancel")
            {
                writer.WriteLine("Module selection canceled.");
                return new List<string>();
            }
        }
    }
}