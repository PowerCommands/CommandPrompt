using PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Contracts;
using Spectre.Console;

namespace PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.DomainObjects;
public class SpectreInfoPanel(IInfoPanelContent content, Color borderColor, Color textColor) : IInfoPanel
{
    private readonly IConsoleWriter _writer = SpectreConsoleWriter.Instance;
    private readonly Color _borderColor = borderColor;
    private readonly Color _textColor = textColor;

    public void Draw(int margin)
    {
        var top = Console.CursorTop;
        var left = Console.CursorLeft;
        Clear(margin);

        var panel = new Panel(new Markup($"[{_textColor}]{content.GetText()}[/]"))
        {
            Border = BoxBorder.Rounded,  // Här använder vi ASCII istället för Unicode
            Padding = new Padding(0, 0),
            BorderStyle = new Style(_borderColor)
        };

        AnsiConsole.Write(panel);
        Console.CursorTop = top;
        Console.CursorLeft = left;
    }

    private void Clear(int margin)
    {
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < margin; i++)
        {
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }
        Console.SetCursorPosition(0, 0);
    }
}
