﻿using Spectre.Console;
using System.Runtime.CompilerServices;
using static Serilog.Log;

namespace PainKiller.CommandPrompt.CoreLib.Core.Presentation;
public class SpectreConsoleWriter : IConsoleWriter
{
    public void WriteDescription(string label, string text, bool writeToLog = true, Color? consoleColor = null, string scope = "")
    {
        var color = consoleColor ?? Color.Blue;
        var panel = new Panel(new Markup($"[{color}]{label}[/] : [grey]{text}[/]"))
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1),
            Header = new PanelHeader("[green]Description[/]", Justify.Center)
        };

        AnsiConsole.Write(panel);

        if (writeToLog)
            Information($"{label} : {text}", scope);
    }
    public void Write(string text, bool writeLog = true, Color? consoleColor = null, [CallerMemberName] string scope = "")
    {
        var color = consoleColor ?? Color.Black;
        var escaped = Markup.Escape(text);
        AnsiConsole.Markup($"{ToDefaultColorIfBlack(escaped, color)}");
        if (writeLog) Information("{Scope}: {Text}", scope, text);
    }

    public void WriteLine(string text = "", bool writeLog = true, Color? consoleColor = null, [CallerMemberName] string scope = "")
    {
        var color = consoleColor ?? Color.Black;
        var escaped = Markup.Escape(text);
        AnsiConsole.MarkupLine($"{ToDefaultColorIfBlack(escaped, color)}");
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

    public void WriteHeadLine(string text, bool writeLog = true, [CallerMemberName] string scope = "")
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
    public void WriteTable<T>(IEnumerable<T> items, string[]? columnNames = null, Color? consoleColor = null)
    {
        var color = consoleColor ?? Color.DarkSlateGray1;
        var table = new Table().Expand().Border(TableBorder.Rounded).BorderColor(Color.DarkSlateGray3);
        var properties = typeof(T).GetProperties();
        if (columnNames != null && columnNames.Length == properties.Length)
        {
            foreach (var columnName in columnNames)
            {
                table.AddColumn(new TableColumn($"[bold {color}]{columnName}[/]").Centered());
            }
        }
        else
        {
            foreach (var property in properties)
            {
                table.AddColumn(new TableColumn($"[bold {color}]{property.Name}[/]").Centered());
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
    private string ToDefaultColorIfBlack(string text, Color color) => color == Color.Black ? text : $"[{color}]{text}[/]";
}

