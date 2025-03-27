using PainKiller.CommandPrompt.CoreLib.Configuration.Contracts;
using PainKiller.ReadLine.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Core.Runtime;

public class CommandLoop(CommandRuntime runtime, IUserInputReader inputReader, ICoreConfiguration config)
{
    public void Start()
    {
        while (true)
        {
            var input = inputReader.ReadLine(config.Prompt).Trim();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            var result = runtime.Execute(input);
        }
    }
}