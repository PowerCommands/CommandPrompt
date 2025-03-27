using System.Text.RegularExpressions;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.PromptKit.Bootstrap;
using Spectre.Console;

namespace PainKiller.PromptKit.Commands;

[CommandDesign("Shows the latest log entries", options: ["hejsan","hoppsan"])]
public class LogCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var logDir = Configuration.Log.FilePath;
        var logFilePrefix = Path.GetFileNameWithoutExtension(Configuration.Log.FileName);

        if (!Directory.Exists(logDir))
            return Nok("Log directory missing.");

        var latestFile = Directory.GetFiles(logDir, $"{logFilePrefix}*.log").OrderByDescending(File.GetLastWriteTime).FirstOrDefault();

        if (latestFile is null)
            return Nok("No log file found.");

        AnsiConsole.MarkupLine($"[bold]Latest log:[/] {Path.GetFileName(latestFile)}");
        var lines = SafeReadLines(latestFile);
        var logEntries = lines
            .Select(ParseLogLine)
            .Select(parsed => new LogEntry { Timestamp = parsed.Timestamp, Level = parsed.Level, Message = parsed.Message }).ToList();

        System.Console.Clear();
        bool LogEntryFilter(LogEntry entry, string filter) => entry.Timestamp.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || entry.Level.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || entry.Message.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        InteractiveFilter<LogEntry>.Run(logEntries, LogEntryFilter, DisplayTable);
        return Ok();
    }
    private (string Timestamp, string Level, string Message) ParseLogLine(string line)
    {
        var match = Regex.Match(line, @"^\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) (\w+)\]\s+(.+)$");
        if (!match.Success) return ("-", "-", line);
        return (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
    }
    private List<string> SafeReadLines(string path)
    {
        var lines = new List<string>();
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream) lines.Add(reader.ReadLine()!);
        return lines;
    }
    private void DisplayTable(IEnumerable<LogEntry> entries)
    {
        var table = new Table().RoundedBorder().AddColumn("[grey]Timestamp[/]").AddColumn("[grey]Level[/]").AddColumn("[grey]Message[/]");

        foreach (var entry in entries)
        {
            var color = entry.Level switch
            {
                "INF" => "green",
                "WRN" => "yellow",
                "ERR" => "red",
                "DBG" => "grey",
                _ => "white"
            };

            table.AddRow(new Markup($"[grey]{Markup.Escape(entry.Timestamp)}[/]"), new Markup($"[{color}]{Markup.Escape(entry.Level)}[/]"), new Markup(Markup.Escape(entry.Message)));
        }
        AnsiConsole.Write(table);
    }
    private class LogEntry
    {
        public string Timestamp { get; init; } = "";
        public string Level { get; init; } = "";
        public string Message { get; init; } = "";
    }
}