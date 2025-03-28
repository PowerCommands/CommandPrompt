using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ReadLine.Managers;
using Spectre.Console;

namespace PainKiller.CommandPrompt.CoreLib.Modules.ShellModule;

[CommandDesign(
    description: "List the content of the current directory or a target directory, optionally open with File Explorer or show drive info.",
    options: ["filter", "browse", "drive-info"],
    arguments: ["Path to directory (optional)"],
    examples:
    [
        "//List content of current directory", "dir",
        "//List and open current directory", "dir --browse",
        "//Show drive info", "dir --drive-info",
        "//List contents of AppData folder", "dir --filter AppData"
    ]
)]
public class DirCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        if (input.Options.ContainsKey("drive-info")) return ShowDriveInfo();

        var path = input.Arguments.FirstOrDefault() ?? Environment.CurrentDirectory;
        if (!path.Contains('\\') && !Path.IsPathRooted(path))
            path = Path.Combine(Environment.CurrentDirectory, path);

        path = Path.GetFullPath(path);
        if (!Directory.Exists(path)) return Nok($"Directory not found: {path}");

        if (input.Options.ContainsKey("browse"))
            new ShellService().OpenDirectory(path);

        Environment.CurrentDirectory = path;
        var filter = input.Options.TryGetValue("filter", out var f) ? f : string.Empty;

        var entries = GetDirectoryEntries(filter);
        SuggestionProviderManager.AppendContextBoundSuggestions(Identifier, entries.Select(e => e.Name).ToArray());

        Console.Clear();
        InteractiveFilter<DirEntry>.Run(entries, EntryFilter, DisplayTable);
        return Ok();
    }

    private List<DirEntry> GetDirectoryEntries(string filter)
    {
        var dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
        var entries = new List<DirEntry>();

        foreach (var dir in dirInfo.GetDirectories().Where(d => d.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)))
        {
            entries.Add(new DirEntry
            {
                Name = dir.Name,
                Type = "<DIR>",
                Size = dir.GetDirectorySize().GetDisplayFormattedFileSize(),
                Updated = dir.LastWriteTime.GetDisplayTimeSinceLastUpdate()
            });
        }

        foreach (var file in dirInfo.GetFiles().Where(f => f.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)))
        {
            entries.Add(new DirEntry
            {
                Name = file.Name,
                Type = file.GetFileTypeDescription(),
                Size = file.Length.GetDisplayFormattedFileSize(),
                Updated = file.LastWriteTime.GetDisplayTimeSinceLastUpdate()
            });
        }

        return entries;
    }

    private bool EntryFilter(DirEntry entry, string filter)
    {
        return entry.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || entry.Type.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private void DisplayTable(IEnumerable<DirEntry> entries)
    {
        var table = new Table()
            .Expand() // <--- gör att tabellen fyller hela konsolfönstret
            .RoundedBorder()
            .AddColumn(new TableColumn("[grey]Name[/]").NoWrap().Width(40))
            .AddColumn(new TableColumn("[grey]Type[/]").Centered())
            .AddColumn(new TableColumn("[grey]Size[/]").RightAligned())
            .AddColumn(new TableColumn("[grey]Updated[/]").RightAligned());

        foreach (var entry in entries)
        {
            var color = entry.Type == "<DIR>" ? "yellow" : "white";
            table.AddRow(
                new Markup($"[{color}]{Markup.Escape(entry.Name)}[/]"),
                new Markup($"[blue]{Markup.Escape(entry.Type)}[/]"),
                new Markup(Markup.Escape(entry.Size)),
                new Markup(Markup.Escape(entry.Updated))
            );
        }

        AnsiConsole.Write(table);
    }


    private RunResult ShowDriveInfo()
    {
        foreach (var drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady) continue;

            AnsiConsole.MarkupLine($"[bold]Drive:[/] {drive.Name}");
            AnsiConsole.MarkupLine($"  Type       : {drive.DriveType}");
            AnsiConsole.MarkupLine($"  Label      : {drive.VolumeLabel}");
            AnsiConsole.MarkupLine($"  Format     : {drive.DriveFormat}");
            AnsiConsole.MarkupLine($"  Total size : {drive.TotalSize.GetDisplayFormattedFileSize()}");
            AnsiConsole.MarkupLine($"  Free space : {drive.TotalFreeSpace.GetDisplayFormattedFileSize()}");
            Console.WriteLine();
        }
        return Ok();
    }

    private class DirEntry
    {
        public string Name { get; init; } = "";
        public string Type { get; init; } = "";
        public string Size { get; init; } = "";
        public string Updated { get; init; } = "";
    }
}