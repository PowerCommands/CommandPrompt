using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;

namespace PainKiller.CommandPrompt.CoreLib.Core.Runtime;

public class CommandExecutor
{ 
    private readonly ILogger<CommandExecutor> _logger = LoggerProvider.CreateLogger<CommandExecutor>();

    public RunResult Execute(IConsoleCommand? command, string rawInput)
    {
        if (command == null)
        {
            _logger.LogError($"Command {command} not found");
            return new RunResult("", "Command not found", "RunResultStatus.NotFound");
        }
        try
        {
            command.Run();
            _logger.LogDebug("This is a debug log");
            return new RunResult(command.Identifier, "Success", "RunResultStatus.Ok");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new RunResult(command.Identifier, ex.Message, "RunResultStatus.ExceptionThrown");
        }
    }
}