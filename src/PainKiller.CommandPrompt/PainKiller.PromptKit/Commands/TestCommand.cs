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
        Console.WriteHeaderLine("Test command");
        Console.WriteCritical("Critical");
        Console.WriteError("Error");
        Console.WriteWarning("Warning");
        return Ok();
    }
}