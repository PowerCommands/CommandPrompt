using PainKiller.CommandPrompt.CoreLib.Configuration.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Core.Runtime;

public class CommandLoop(CommandRuntime runtime, ICoreConfiguration config)
{
    public void Start()
    {
        while (true)
        {
            Console.Write(config.Prompt);
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            var result = runtime.Execute(input);
        }
    }
}