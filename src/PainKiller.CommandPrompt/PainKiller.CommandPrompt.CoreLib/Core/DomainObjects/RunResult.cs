namespace PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;

public class RunResult(string executingCommand, string input, string output)
{
    public string ExecutingCommand { get; } = executingCommand;
    public string Input { get; } = input;
    public string Output { get; } = output;
}