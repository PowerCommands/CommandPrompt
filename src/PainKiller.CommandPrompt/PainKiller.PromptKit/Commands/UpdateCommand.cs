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

[CommandDesign(description:"Update your modules in your CommandPrompt project.", 
                  examples: ["//Update project, you can use cd command and tab to navigate to the projects root directory","update <rootDirectory to project>"])]
public class UpdateCommand : ConsoleCommandBase<CommandPromptConfiguration>
{
    private void OnWorkingDirectoryChanged(WorkingDirectoryChangedEventArgs e) => UpdateSuggestions(e.NewWorkingDirectory);
    public UpdateCommand(string identifier) : base(identifier)  => EventBusService.Service.Subscribe<WorkingDirectoryChangedEventArgs>(OnWorkingDirectoryChanged);
    public override RunResult Run(ICommandLineInput input)
    {
        var outputPath = input.GetFullPath();
        var updateManager = new UpdateManager(Configuration.PromptKit.UpdateFilename, outputPath, Writer);
        updateManager.Update();
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
}