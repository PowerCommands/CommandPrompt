using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Services;

namespace PainKiller.CommandPrompt.CoreLib.Core.Runtime;

public class CommandRuntime
{
    private readonly Dictionary<string, IConsoleCommand> _commands;
    private readonly CommandExecutor _executor;

    public CommandRuntime()
    {
        _commands = CommandDiscoveryService.DiscoverCommands().ToDictionary(c => c.Identifier, StringComparer.OrdinalIgnoreCase);
        _executor = new CommandExecutor();
    }

    public RunResult Execute(string input)
    {
        var identifier = input.Trim().Split(' ')[0];
        var command = _commands.GetValueOrDefault(identifier);
        return _executor.Execute(command, input);
    }
}