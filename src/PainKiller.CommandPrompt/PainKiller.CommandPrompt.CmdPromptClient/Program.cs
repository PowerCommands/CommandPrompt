using PainKiller.CommandPrompt.CmdPromptClient.Bootstrap;
using PainKiller.CommandPrompt.CoreLib.Core.Runtime;
using PainKiller.CommandPrompt.CoreLib.Core.Services;

var config = Startup.ConfigureServices();
var commands = CommandDiscoveryService.DiscoverCommands(config);
new CommandLoop(new CommandRuntime(commands), config).Start();
