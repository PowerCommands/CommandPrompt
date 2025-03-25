using PainKiller.CommandPrompt.CmdPromptClient.Bootstrap;
using PainKiller.CommandPrompt.CoreLib.Core.Runtime;
Startup.ConfigureServices();
new CommandLoop(new CommandRuntime()).Start();
