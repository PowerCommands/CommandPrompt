using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Modules.TextModule.Utility;

namespace PainKiller.CommandPrompt.CoreLib.Modules.TextModule.Commands;
public class TextCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var resultText = TextInputHelper.CaptureText();

        if (string.IsNullOrWhiteSpace(resultText))
        {
            Writer.WriteLine("No text was entered.");
            return Nok("No text captured.");
        }
        Writer.WriteLine(resultText);
        return Ok();
    }
}
