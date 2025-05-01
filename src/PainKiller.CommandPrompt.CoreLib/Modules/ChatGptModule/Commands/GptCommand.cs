using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;

namespace PainKiller.CommandPrompt.CoreLib.Modules.ChatGptModule.Commands;

public class GptCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        ShellService.Default.OpenWithDefaultProgram(Configuration.Core.Modules.ChatGpt.SearchUri.Replace("$QUERY$", string.Join(" ", input.Arguments)));
        return Ok();
    }
}