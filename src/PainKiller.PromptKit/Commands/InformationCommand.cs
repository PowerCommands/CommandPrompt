using System.Text.RegularExpressions;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Configuration;
using PainKiller.PromptKit.Extensions;
using PainKiller.PromptKit.Managers;
using Spectre.Console;

namespace PainKiller.PromptKit.Commands;

[CommandDesign(     description: "Show information about the Command Prompt project", 
                       examples: ["//Show information","information"])]
public class InformationCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var coreProjectRoot = Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).GetRoot().FullName, "PainKiller.CommandPrompt.CoreLib");
        var informationManager = new InformationManager(coreProjectRoot);
        var totalNumberOfFiles = informationManager.GetTotalNumberOfFiles();
        var totalNumberOfCodeLines = informationManager.GetTotalNumberOfCodeLines();
        var thirdPartyComponents = informationManager.GetThirdPartyComponents();
        var modules = new ModuleManager(Path.Combine(coreProjectRoot, "Modules"), Writer).ModulesDiscovery();

        Writer.WriteHeadLine("Information about the Command Prompt project");
        Writer.WriteLine("Metrics (Modules excluded)");
        DisplayProjectStatistics(totalNumberOfFiles, totalNumberOfCodeLines);
        Writer.WriteLine("Dependencies");
        DisplayThirdPartyComponents(thirdPartyComponents);
        Writer.WriteLine("Modules");
        DisplayThirdPartyComponents(modules);

        return Ok();
    }

    private void DisplayProjectStatistics(int totalFiles, int totalLines)
    {
        var data = new List<ProjectStatistics>
        {
            new() { Metric = "Total number of files", Value = totalFiles.ToString() },
            new() { Metric = "Total number of code lines", Value = totalLines.ToString() }
        };

        var columnNames = new[] { "Metric", "Value" };
        Writer.WriteTable(data, columnNames, borderColor: Color.Magenta3, expand: false);
    }
    private void DisplayThirdPartyComponents(List<(string Name, string Version)> components)
    {
        if (!components.Any())
        {
            Writer.WriteLine("No third-party components found.");
            return;
        }

        var versionPattern = new Regex(@"^\d+\.\d+(\.\d+)?$");

        var data = components.Select(c =>
        {
            var version = versionPattern.IsMatch(c.Version) ? c.Version : ExtractVersionFromText(c.Version);
            return new ThirdPartyComponent { Name = c.Name, Version = version };
        }).ToList();

        var columnNames = new[] { "Name", "Version" };
        Writer.WriteTable(data, columnNames, borderColor: Color.Magenta3, expand: false);
    }
    private string ExtractVersionFromText(string text)
    {
        var versionPattern = new Regex(@"Version\s+(\d+\.\d+(\.\d+)?)", RegexOptions.IgnoreCase);
        var match = versionPattern.Match(text);
        return match.Success ? match.Groups[1].Value : "Unknown";
    }
    public class ProjectStatistics
    {
        public string Metric { get; set; } = String.Empty;
        public string Value { get; set; } = String.Empty;
    }
    public class ThirdPartyComponent
    {
        public string Name { get; set; } = String.Empty;
        public string Version { get; set; } = String.Empty;
    }
}