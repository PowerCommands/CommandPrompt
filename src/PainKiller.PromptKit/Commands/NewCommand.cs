using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Configuration;
using PainKiller.PromptKit.Managers;

namespace PainKiller.PromptKit.Commands;


[CommandDesign(description:  "Create new CommandPrompt project", 
                  examples: ["//Create new project","new",
                             "//Create new project with name Company.MagicPrompts","new Company.MagicPrompts"])]
public class NewCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    
    public override RunResult Run(ICommandLineInput input)
    {
        var projectName = input.Arguments.FirstOrDefault();
        if(string.IsNullOrEmpty(projectName)) projectName = DialogService.QuestionAnswerDialog("Name your project, a prefix is recommended, like Company.MagicPrompts: ");
        var defaultPath = string.IsNullOrEmpty(Configuration.PromptKit.OutputPath) ? Path.GetTempPath() : Configuration.PromptKit.OutputPath;
        var outputDirectory = DialogService.PathDialog("Where do you want to output your new project? \n(a directory with the project name will be created in output folder)", Path.Combine(defaultPath, nameof(PromptKit), projectName));
        CommandDiscoveryService.TryGetCommand("cd", out var cdCommand);
        cdCommand!.Execute(options: new Dictionary<string, string> { { "modules", "" },{ "no-output", "" } });
        var templateManager = new InstallManager(projectName, Environment.CurrentDirectory, outputDirectory, Configuration.PromptKit.ConfigurationTemplate, Configuration.PromptKit.Ignores, Writer);
        templateManager.Install();
        return Ok();
    }
}