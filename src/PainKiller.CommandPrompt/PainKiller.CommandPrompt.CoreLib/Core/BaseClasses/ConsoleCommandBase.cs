using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;

namespace PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;

public class ConsoleCommandBase(string identifier) : IConsoleCommand
{
    public string Identifier { get; } = identifier;

    public bool InitializeAndValidateInput(string input)
    {
        throw new NotImplementedException();
    }

    public void RunCompleted()
    {
        throw new NotImplementedException();
    }

    public virtual RunResult Run()
    {
        Console.WriteLine("OK");
        return new RunResult(nameof(ConsoleCommandBase), "input", "output");
    }
}