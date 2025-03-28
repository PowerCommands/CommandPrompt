using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Bootstrap;

namespace PainKiller.PromptKit.Commands;

[CommandDesign("Displays Hello", options: ["Word","Banan"])]
public class WordCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        Writer.WriteLine($"Hello {string.Join(",", input.Arguments)} {string.Join(",", input.Quotes)} {string.Join(",", input.Options.Values)}");
        return Ok();
    }
}