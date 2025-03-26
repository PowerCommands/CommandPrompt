using PainKiller.CommandPrompt.CmdPromptClient.Bootstrap;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;

namespace PainKiller.CommandPrompt.CmdPromptClient.Commands;

public class HelloCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        Console.WriteLine($"Hello {string.Join(",", input.Arguments)}");
        Console.WriteSuccessLine(Configuration.Log.FileName);
        return Ok();
    }
}