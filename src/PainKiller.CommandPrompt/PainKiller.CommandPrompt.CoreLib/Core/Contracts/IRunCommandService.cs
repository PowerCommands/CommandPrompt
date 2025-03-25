using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;

namespace PainKiller.CommandPrompt.CoreLib.Core.Contracts;

public interface IRunCommandService
{
    RunResult Run(string commandName, string input);
    IConsoleCommand GetCommand(string commandName);
}