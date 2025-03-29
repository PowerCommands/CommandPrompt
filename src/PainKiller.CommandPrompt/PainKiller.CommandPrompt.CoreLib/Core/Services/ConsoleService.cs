namespace PainKiller.CommandPrompt.CoreLib.Core.Services;

public static class ConsoleService
{
    public static readonly IConsoleWriter Writer = new SpectreConsoleWriter();
    public static void WriteCenteredText(string text)
    {
        var consoleWidth = Console.WindowWidth;
        var padding = (consoleWidth - text.Length) / 2;
        var centeredText = text.PadLeft(padding + text.Length).PadRight(consoleWidth);
        ConsoleService.Writer.WriteLine(centeredText, writeLog: false, ConsoleColor.DarkMagenta);
    }
}