using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.PromptKit.Bootstrap;

namespace PainKiller.PromptKit.Commands;

public class HelloCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        Console.WriteLine($"Hello {string.Join(",", input.Arguments)}");
        Console.WriteSuccessLine(Configuration.Log.FileName);
        return Ok();
    }
}