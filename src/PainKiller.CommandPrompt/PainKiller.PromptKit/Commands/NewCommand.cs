using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Bootstrap;
using PainKiller.PromptKit.Managers;

namespace PainKiller.PromptKit.Commands;


[CommandDesign("Just a dumie test Commando", suggestions: ["förlag1","förslag2"])]
public class NewCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    
    public override RunResult Run(ICommandLineInput input)
    {
        var publisherManager = new PublisherManager(Environment.CurrentDirectory);
        publisherManager.Run();
        return Ok();
    }
}