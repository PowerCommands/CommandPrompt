using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Events;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Configuration;
using PainKiller.PromptKit.Managers;
using PainKiller.ReadLine.Managers;

namespace PainKiller.PromptKit.Commands;

[CommandDesign(description:  "Update your modules or your core project.", 
                   options: ["core"],
                  examples: ["//Update project, you can use cd command and tab to navigate to the projects root directory","update <rootDirectory to project>"])]
public class UpdateCommand : ConsoleCommandBase<CommandPromptConfiguration>
{
    private void OnWorkingDirectoryChanged(WorkingDirectoryChangedEventArgs e) => UpdateSuggestions(e.NewWorkingDirectory);
    public UpdateCommand(string identifier) : base(identifier)  => EventBusService.Service.Subscribe<WorkingDirectoryChangedEventArgs>(OnWorkingDirectoryChanged);
    public override RunResult Run(ICommandLineInput input)
    {
        var outputPath = input.GetFullPath();
        if (input.HasOption("core")) return UpdateCoreProject(outputPath);

        Writer.WriteHeadLine("Update info.");
        Writer.WriteDescription("Project to update:", outputPath);
        var modulePath = Path.Combine(outputPath, "PainKiller.CommandPrompt.CoreLib", "Modules");
        var moduleManager = new ModuleManager(modulePath, Writer);
        Writer.WriteDescription("Modules installed:", string.Join(',', moduleManager.ModulesInstalled()));
        Writer.WriteDescription("Nice to know?:", "You can add new modules, but you may have to do some manual steps to fully implement the new module.");

        var updateManager = new UpdateManager(Configuration.PromptKit.UpdateFilename, outputPath, Writer);
        updateManager.Update();
        Writer.WriteDescription("Added new modules, instead of update?",$"New modules can have configuration values that you may need to add to the configuration file. (or use defaults)\nYou also need to add the Configuration Property to {nameof(ModulesConfiguration)}.cs class file in the Core project.\nA module can have dependencies, look in the README.md file.");
        return Ok();
    }
    private void UpdateSuggestions(string newWorkingDirectory)
    {
        if (!Directory.Exists(newWorkingDirectory)) return;
        var directories = Directory.GetDirectories(newWorkingDirectory)
            .Select(d => new DirectoryInfo(d).Name)
            .ToArray();
        SuggestionProviderManager.AppendContextBoundSuggestions(Identifier, directories.Select(e => e).ToArray());
    }
    private RunResult UpdateCoreProject(string targetPath)
    {
        Writer.WriteHeadLine("Update CoreLib Files");
        Writer.WriteDescription("Project root path:", targetPath);
        var manager = new UpdateCoreManager(targetPath, Writer);
        manager.Update();
        return Ok();
    }
}