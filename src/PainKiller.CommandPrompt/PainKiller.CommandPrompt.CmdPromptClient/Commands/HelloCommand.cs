using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;

namespace PainKiller.CommandPrompt.CmdPromptClient.Commands;

public class HelloCommand(string identifier) : ConsoleCommandBase(identifier)
{
    public override RunResult Run()
    {
        //Console.WriteHeaderLine("This is the only command that exist so far!");
        Console.WriteLine("Hello world!");
        return new RunResult("ok", "","");
    }
}