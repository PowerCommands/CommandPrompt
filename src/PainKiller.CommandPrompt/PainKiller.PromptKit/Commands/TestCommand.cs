using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Bootstrap;

namespace PainKiller.PromptKit.Commands;


[CommandDesign("Just a dumie test Commando", suggestions: ["förlag1","förslag2"])]
public class TestCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        Writer.WriteHeaderLine("Test command");
        Writer.WriteCritical("Critical");
        Writer.WriteError("Error");
        Writer.WriteWarning("Warning");
        Writer.WriteDescription("Config",Configuration.RoamingDirectoryName);
        return Ok();
    }
}