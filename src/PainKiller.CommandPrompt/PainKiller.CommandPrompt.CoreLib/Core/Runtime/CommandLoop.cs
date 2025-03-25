namespace PainKiller.CommandPrompt.CoreLib.Core.Runtime;

public class CommandLoop(CommandRuntime runtime)
{
    public void Start()
    {
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            var result = runtime.Execute(input);
            Console.WriteLine($"[{result.Output}] {result.Output}");
        }
    }
}