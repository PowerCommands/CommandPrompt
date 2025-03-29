using Spectre.Console;
using System.Runtime.CompilerServices;
using static Serilog.Log;

namespace PainKiller.CommandPrompt.CoreLib.Core.Presentation;
public class SpectreConsoleWriter : IConsoleWriter
{
    public void WriteDescription(string label, string text, bool writeToLog = true, ConsoleColor consoleColor = ConsoleColor.Gray, string scope = "")
    {
        var panel = new Panel(new Markup($"[blue]{label}[/] : [grey]{text}[/]"))
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1),
            Header = new PanelHeader("[green]Description[/]", Justify.Center)
        };

        AnsiConsole.Write(panel);

        if (writeToLog)
            Information($"{label} : {text}", scope);
    }
    public void Write(string text, bool writeLog = true, ConsoleColor color = ConsoleColor.Black, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.Markup($"{WrapColor(escaped, color)}");
        if (writeLog) Information("{Scope}: {Text}", scope, text);
    }

    public void WriteLine(string text = "", bool writeLog = true, ConsoleColor color = ConsoleColor.Black, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"{WrapColor(escaped, color)}");
        if (writeLog) Information("{Scope}: {Text}", scope, text);
    }

    public void WriteSuccessLine(string text, bool writeLog = true, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"[green]{escaped}[/]");
        if (writeLog) Information("{Scope}: {Text}", scope, text);
    }

    public void WriteWarning(string text, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"[yellow]{escaped}[/]");
        Warning("{Scope}: {Text}", scope, text);
    }

    public void WriteError(string text, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"[red]{escaped}[/]");
        Error("{Scope}: {Text}", scope, text);
    }

    public void WriteCritical(string text, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"[bold red]{escaped}[/]");
        Fatal("{Scope}: {Text}", scope, text);
    }

    public void WriteHeaderLine(string text, bool writeLog = true, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"[bold blue]{escaped}[/]");
        if (writeLog) Information("{Scope}: {Text}", scope, text);
    }

    public void WriteUrl(string text, bool writeLog = true, [CallerMemberName] string scope = "")
    {
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"[underline blue]{escaped}[/]");
        if (writeLog) Information("{Scope} [URL]: {Text}", scope, text);
    }

    public void WritePrompt(string prompt)
    {
        var escaped = Markup.Escape(prompt);
        AnsiConsole.Markup($"[bold]{escaped} [/]");
    }

    public void Clear() => AnsiConsole.Clear();
    public void WriteTable<T>(IEnumerable<T> items, string[]? columnNames = null, ConsoleColor columnColor = ConsoleColor.DarkMagenta)
    {
        var table = new Table().Expand().Border(TableBorder.Rounded).BorderColor(Color.DarkSlateGray3);
        var properties = typeof(T).GetProperties();
        if (columnNames != null && columnNames.Length == properties.Length)
        {
            foreach (var columnName in columnNames)
            {
                table.AddColumn(new TableColumn($"[bold {columnColor}]{columnName}[/]").Centered());
            }
        }
        else
        {
            foreach (var property in properties)
            {
                table.AddColumn(new TableColumn($"[bold {columnColor}]{property.Name}[/]").Centered());
            }
        }
        foreach (var item in items)
        {
            var row = new List<Markup>();

            foreach (var property in properties)
            {
                row.Add(new Markup(Markup.Escape(property.GetValue(item)?.ToString() ?? string.Empty)));
            }
            table.AddRow(row.ToArray());
        }
        AnsiConsole.Write(table);
    }
    private string WrapColor(string text, ConsoleColor color)
    {
        if (color == ConsoleColor.Black) return text;
        var markup = ToMarkup(color);
        return string.IsNullOrEmpty(markup) ? text : $"[{markup}]{text}[/]";
    } private string ToMarkup(ConsoleColor color) => color switch
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
        ConsoleColor.DarkMagenta => "magenta",
        ConsoleColor.DarkGray => "brightwhite",
        ConsoleColor.Black => "black",
        _ => string.Empty
    };
}

