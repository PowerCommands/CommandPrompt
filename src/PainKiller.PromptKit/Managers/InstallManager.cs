using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.Events;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
using PainKiller.PromptKit.Configuration;
using PainKiller.PromptKit.DomainObjects;
using Spectre.Console;

namespace PainKiller.PromptKit.Managers;

public class InstallManager(string projectName, string modulesDirectory, string outputDirectory, string configurationTemplateName, List<string> ignores, IConsoleWriter writer)
{
    private readonly ILogger<InstallManager> _logger = LoggerProvider.CreateLogger<InstallManager>();
    private readonly ModuleManager _moduleManager = new ModuleManager(modulesDirectory, writer);
    public void Install()
    {
        var paths = new TemplatePaths(modulesDirectory, outputDirectory, projectName, configurationTemplateName);
        Environment.CurrentDirectory = AppContext.BaseDirectory;    //Secures that you don´t write to directory in your PromptKit project.
        var modules = _moduleManager.ModulesDiscovery();
        _logger.LogDebug($"Modules found: {string.Join(',', modules)}");
        var selectedModules = _moduleManager.DisplayModuleSelection(modules);

        var confirm = ShowConfirmationDialog(projectName, selectedModules, outputDirectory);
        if (!confirm) return;

        if (Directory.Exists(paths.Root.Target)) Directory.Delete(outputDirectory, recursive: true);
        var copyManager = new CopyManager(paths);
        copyManager.CopyCoreProject(selectedModules, ignores);
        writer.WriteSuccessLine("✅ Copy Core project");

        var configurationFileCreator = new ConfigurationTemplateManager(paths, projectName);
        configurationFileCreator.CreateYamlConfigurationFile(selectedModules.ToList());
        writer.WriteSuccessLine($"✅ {nameof(CommandPromptConfiguration)}.yaml file created.");
        configurationFileCreator.ProcessCsConfiguration(paths.ModulesConfigurationPath.Source, paths.ModulesConfigurationPath.Target, selectedModules);
        writer.WriteSuccessLine($"✅ {nameof(ModulesConfiguration)}.cs file created.");


        var appCreationManager = new AppProjectCreationManager(paths, projectName);
        appCreationManager.CreateAppProject(ignores);
        writer.WriteSuccessLine($"✅ {projectName} created.");

        IOService.CopyFolder(paths.ReadLineRoot.Source, paths.ReadLineRoot.Target);
        writer.WriteSuccessLine($"✅ ReadLine project copied.");

        appCreationManager.CreateSolutionFile();
        writer.WriteSuccessLine($"✅ VS Solution file created.");

        var dependencyManager = new DependencyManager(Path.Combine(paths.CoreLibRoot.Target, $"PainKiller.CommandPrompt.CoreLib.csproj"), paths.CoreLibRoot.Source, selectedModules, modules.Select(m => m.Name), writer);
        dependencyManager.CleanDependencies();

        writer.WriteLine();
        writer.WriteHeadLine("Everything is created!");

        Environment.CurrentDirectory = outputDirectory;
        EventBusService.Service.Publish(new WorkingDirectoryChangedEventArgs(Environment.CurrentDirectory));
        ShellService.Default.OpenDirectory(Environment.CurrentDirectory);
    }
    
    private bool ShowConfirmationDialog(string name, List<string> selectedModules, string outputDir)
    {
        var projectNameDisplay = $"[bold blue]{name}[/]";
        var selectedModulesDisplay = selectedModules.Any()
            ? string.Join(", ", selectedModules.Select(m => $"[bold gray]{m}[/]"))
            : "[bold red]None[/]";
        var outputPathDisplay = $"[bold magenta]{outputDir}[/]";

        var infoPanel = new Panel(
                new Markup(
                    $"[bold]Project Name:[/] {projectNameDisplay}\n" +
                    $"[bold]Selected Modules:[/] {selectedModulesDisplay}\n" +
                    $"[bold]Output Path:[/] {outputPathDisplay}\n\n" +
                    "[bold red]Warning:[/] The output path will be [bold]deleted[/] if it already exists."
                ))
            .Header("[DarkMagenta]Project Configuration[/]")
            .Border(BoxBorder.Double)
            .BorderStyle(Style.Parse("Gray"))
            .Padding(1, 1);

        AnsiConsole.Write(infoPanel);

        return AnsiConsole.Confirm("[gray]Do you want to proceed?[/]");
    }


}