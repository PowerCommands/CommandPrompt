using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;
using PainKiller.PromptKit.Configuration;

namespace PainKiller.PromptKit.Commands;

[CommandDesign("Test what happends when a command throws an Exception.", options: ["name","foo"])]
public class FailureCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var secret = Configuration.Core.Modules.Security.DecryptSecret("babar");
        Writer.WriteSuccessLine(secret);
        File.WriteAllText("ökafds", "lkjadfs");
        return Ok();
    }
}