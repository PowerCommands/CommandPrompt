using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using Serilog;
using Spectre.Console;
using System.Runtime.CompilerServices;

namespace PainKiller.CommandPrompt.CoreLib.Core.Presentation;
public class SpectreConsoleWriter() : IConsoleWriter
{
    public void Write(string text, bool writeLog = true, ConsoleColor color = ConsoleColor.Black, [CallerMemberName] string scope = "")
    {
        AnsiConsole.Markup($"{WrapColor(text, color)}");
        if (writeLog) Log.Information("{Scope}: {Text}", scope, text);
    }
    public void WriteLine(string text, bool writeLog = true, ConsoleColor color = ConsoleColor.Black, [CallerMemberName] string scope = "")
    {
        AnsiConsole.MarkupLine($"{WrapColor(text, color)}");
        if (writeLog) Log.Information("{Scope}: {Text}", scope, text);
    }
    public void WriteSuccessLine(string text, bool writeLog = true, [CallerMemberName] string scope = "")
    {
        AnsiConsole.MarkupLine($"[green]{text}[/]");
        if (writeLog) Log.Information("{Scope}: {Text}", scope, text);
    }
    public void WriteWarning(string text, [CallerMemberName] string scope = "")
    {
        AnsiConsole.MarkupLine($"[yellow]{text}[/]");
        Log.Warning("{Scope}: {Text}", scope, text);
    }
    public void WriteError(string text, [CallerMemberName] string scope = "")
    {
        AnsiConsole.MarkupLine($"[red]{text}[/]");
        Log.Error("{Scope}: {Text}", scope, text);
    }
    public void WriteCritical(string text, [CallerMemberName] string scope = "")
    {
        AnsiConsole.MarkupLine($"[bold red]{text}[/]");
        Log.Fatal("{Scope}: {Text}", scope, text);
    }
    public void WriteHeaderLine(string text, bool writeLog = true, [CallerMemberName] string scope = "")
    {
        AnsiConsole.MarkupLine($"[bold blue]{text}[/]");
        if (writeLog) Log.Information("{Scope}: {Text}", scope, text);
    }
    public void WriteUrl(string text, bool writeLog = true, [CallerMemberName] string scope = "")
    {
        AnsiConsole.MarkupLine($"[underline blue]{text}[/]");
        if (writeLog) Log.Information("{Scope} [URL]: {Text}", scope, text);
    }
    public void WritePrompt(string prompt)
    {
        AnsiConsole.Markup($"[bold]{prompt} [/]");
    }
    public void Clear()
    {
        AnsiConsole.Clear();
    }
    private string WrapColor(string text, ConsoleColor color)
    {
        if (color == ConsoleColor.Black) return text;
        var markup = ToMarkup(color);
        return string.IsNullOrEmpty(markup) ? text : $"[{markup}]{text}[/]";
    }
    private string ToMarkup(ConsoleColor color) => color switch
    {
        ConsoleColor.Red => "red",
        ConsoleColor.Green => "green",
        ConsoleColor.Yellow => "yellow",
        ConsoleColor.Blue => "blue",
        ConsoleColor.Cyan => "cyan",
        ConsoleColor.Magenta => "magenta",
        ConsoleColor.White => "white",
        ConsoleColor.Gray => "grey",
        ConsoleColor.DarkRed => "brightred",
        ConsoleColor.DarkGreen => "brightgreen",
        ConsoleColor.DarkYellow => "brightyellow",
        ConsoleColor.DarkBlue => "brightblue",
        ConsoleColor.DarkCyan => "brightcyan",
        ConsoleColor.DarkMagenta => "brightmagenta",
        ConsoleColor.DarkGray => "brightwhite",
        ConsoleColor.Black => "black",
        _ => string.Empty
    };
}
