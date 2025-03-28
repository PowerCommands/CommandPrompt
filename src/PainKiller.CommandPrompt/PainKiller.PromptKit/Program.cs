using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.PromptKit.Bootstrap;

Console.OutputEncoding = System.Text.Encoding.UTF8;
ConsoleService.WriteCenteredText("***** COMMAND PROMPT 1.0 *****");
Console.WriteLine();
Startup.Build().Start();