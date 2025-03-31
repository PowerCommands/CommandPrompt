﻿using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Events;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Core.Utils;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
using PainKiller.PromptKit.Bootstrap;
using PainKiller.PromptKit.DomainObjects;
using Spectre.Console;

namespace PainKiller.PromptKit.Managers;

public class TemplateManager(string projectName, string modulesDirectory, string outputDirectory, string configurationTemplateName, List<string> ignores)
{
    private readonly ILogger<TemplateManager> _logger = LoggerProvider.CreateLogger<TemplateManager>();
    public void Run()
    {
        var paths = new TemplatePaths(modulesDirectory, outputDirectory, projectName, configurationTemplateName);
        Environment.CurrentDirectory = AppContext.BaseDirectory;    //Secures that you don´t write to directory in your PromptKit project.
        var modules = ModulesDiscovery();
        _logger.LogDebug($"Modules found: {string.Join(',', modules)}");
        var selectedModules = DisplayModuleSelection(modules);
        if(Directory.Exists(paths.Root.Target)) Directory.Delete(outputDirectory, recursive: true);
        var copyManager = new CopyManager(paths);
        copyManager.CopyCoreProject(selectedModules, ignores);
        ConsoleService.Writer.WriteSuccessLine("✅ Copy Core project");
        
        var configurationFileCreator = new ConfigurationTemplateManager(paths, projectName);
        configurationFileCreator.CreateYamlConfigurationFile(selectedModules.ToList());
        ConsoleService.Writer.WriteSuccessLine($"✅ {nameof(CommandPromptConfiguration)}.yaml file created.");
        configurationFileCreator.ProcessCsConfiguration(paths.ModulesConfigurationPath.Source, paths.ModulesConfigurationPath.Target, selectedModules);
        ConsoleService.Writer.WriteSuccessLine($"✅ {nameof(ModulesConfiguration)}.cs file created.");


        var appCreationMAnager = new AppProjectCreationManager(paths, projectName);
        appCreationMAnager.CreateAppProject(ignores);
        ConsoleService.Writer.WriteSuccessLine($"✅ {projectName} created.");
        
        IOService.CopyFolder(paths.ReadLineRoot.Source, paths.ReadLineRoot.Target);
        ConsoleService.Writer.WriteSuccessLine($"✅ ReadLine project copied.");
        
        appCreationMAnager.CreateSolutionFile();
        ConsoleService.Writer.WriteSuccessLine($"✅ VS Solution file created.");

        ConsoleService.Writer.WriteLine();
        ConsoleService.Writer.WriteHeaderLine("Everything is created!");

        Environment.CurrentDirectory = outputDirectory;
        EventBusService.Service.Publish(new WorkingDirectoryChangedEventArgs(Environment.CurrentDirectory));
        ShellService.Default.OpenDirectory(Environment.CurrentDirectory);
    }
    private List<(string Name, string Description)> ModulesDiscovery()
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
    private List<string> DisplayModuleSelection(List<(string Name, string Description)> modules)
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

        ConsoleService.Writer.WriteDescription("Selected modules", $"{string.Join(", ", selectedModules.Select(s => s.Split(' ').First()))}");
        return selectedModules.Select(s => s.Split(' ').First()).ToList();
    }
}