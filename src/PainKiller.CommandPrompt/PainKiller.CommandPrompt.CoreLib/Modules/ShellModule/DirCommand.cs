using System.Text.RegularExpressions;
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
    description: "List the content of the current directory or a target directory, optionally open with File Explorer or show drive info.\n" +
                 "Supports interactive filtering by name, type, size or last updated.\n\n" +
                 "Filter expressions:\n" +
                 "- 'log' → files or folders with 'log' in name/type\n" +
                 "- 'size > 100' → larger than 100 MB\n" +
                 "- 'type = image' → jpg, png, gif, etc\n" +
                 "- 'updated < 30d' → modified in last 30 days",
    options: ["browse", "drive-info"],
    arguments: ["Path to directory (optional)"],
    examples:
    [
        "//List current folder", "dir",
        "//Browse current directory", "dir --browse",
        "//Show drives", "dir --drive-info",
        "//Enter directory and filter", "dir C:\\temp"
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

        var entries = GetDirectoryEntries();
        SuggestionProviderManager.AppendContextBoundSuggestions(Identifier, entries.Select(e => e.Name).ToArray());

        Console.Clear();
        InteractiveFilter<DirEntry>.Run(entries, EntryFilter, DisplayTable);
        return Ok();
    }

    private List<DirEntry> GetDirectoryEntries()
    {
        var dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
        var entries = new List<DirEntry>();
        foreach (var dir in dirInfo.GetDirectories())
        {
            entries.Add(new DirEntry
            {
                Name = dir.Name,
                Type = "<DIR>",
                Size = dir.GetDirectorySize().GetDisplayFormattedFileSize(),
                Updated = dir.LastWriteTime.GetDisplayTimeSinceLastUpdate()
            });
        }
        foreach (var file in dirInfo.GetFiles())
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
    if (string.IsNullOrWhiteSpace(filter)) return true;

    filter = filter.Trim();

    // size > N
    var sizeMatch = Regex.Match(filter, @"^size\s*(>|<|=)\s*(\d+(\.\d+)?)$", RegexOptions.IgnoreCase);
    if (sizeMatch.Success)
    {
        var op = sizeMatch.Groups[1].Value;
        var thresholdMb = double.Parse(sizeMatch.Groups[2].Value);
        var bytesMatch = Regex.Match(entry.Size, @"\((\d[\d ]*) bytes\)");
        if (!bytesMatch.Success || !long.TryParse(bytesMatch.Groups[1].Value.Replace(" ", ""), out var bytes))
            return false;
        var sizeInMb = bytes / 1048576.0;

        return op switch
        {
            ">" => sizeInMb > thresholdMb,
            "<" => sizeInMb < thresholdMb,
            "=" => Math.Abs(sizeInMb - thresholdMb) < 0.01,
            _ => false
        };
    }

    // type = category
    var typeMatch = Regex.Match(filter, @"^type\s*=\s*(\w+)$", RegexOptions.IgnoreCase);
    if (typeMatch.Success)
    {
        var category = typeMatch.Groups[1].Value.ToLowerInvariant();
        return category switch
        {
            "image" => IsCategory(entry.Type, ["jpeg", "png", "gif", "bmp", "tiff", "svg", "webp"]),
            "video" => IsCategory(entry.Type, ["mp4", "mkv", "avi", "mov", "wmv", "flv", "webm"]),
            "audio" => IsCategory(entry.Type, ["mp3", "wav", "flac", "aac", "ogg", "m4a"]),
            "code"  => IsCategory(entry.Type, ["c#", "python", "javascript", "html", "css", "java", "php", "cpp", "typescript"]),
            _ => entry.Type.ToLowerInvariant().Contains(category)
        };
    }

    // updated > 3d / updated < 2y
    var updatedMatch = Regex.Match(filter, @"^updated\s*(>|<|=)\s*(\d+)([dmy])$", RegexOptions.IgnoreCase);
    if (updatedMatch.Success)
    {
        var op = updatedMatch.Groups[1].Value;
        var value = int.Parse(updatedMatch.Groups[2].Value);
        var unit = updatedMatch.Groups[3].Value.ToLower();

        var threshold = unit switch
        {
            "d" => DateTime.Now.AddDays(-value),
            "m" => DateTime.Now.AddMonths(-value),
            "y" => DateTime.Now.AddYears(-value),
            _ => DateTime.MinValue
        };

        var entryTime = ParseRelativeTime(entry.Updated);

        return op switch
        {
            ">" => entryTime < threshold,
            "<" => entryTime > threshold,
            "=" => Math.Abs((entryTime - threshold).TotalDays) < 1,
            _ => false
        };
    }

    // fallback: text match
    return entry.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || entry.Type.Contains(filter, StringComparison.OrdinalIgnoreCase);
}

private bool IsCategory(string type, string[] extensions)
{
    return extensions.Any(ext => type.Contains(ext, StringComparison.OrdinalIgnoreCase));
}

private DateTime ParseRelativeTime(string input)
{
    // Exempel: "3 days ago", "2 months ago"
    var match = Regex.Match(input, @"(\d+)\s+(seconds|minutes|hours|days|months|years)", RegexOptions.IgnoreCase);
    if (!match.Success) return DateTime.Now;

    var value = int.Parse(match.Groups[1].Value);
    return match.Groups[2].Value.ToLower() switch
    {
        "seconds" => DateTime.Now.AddSeconds(-value),
        "minutes" => DateTime.Now.AddMinutes(-value),
        "hours" => DateTime.Now.AddHours(-value),
        "days" => DateTime.Now.AddDays(-value),
        "months" => DateTime.Now.AddMonths(-value),
        "years" => DateTime.Now.AddYears(-value),
        _ => DateTime.Now
    };
}


    private void DisplayTable(IEnumerable<DirEntry> entries)
    {
        var table = new Table()
            .Expand()
            .RoundedBorder()
            .AddColumn(new TableColumn("[grey]Name[/]").LeftAligned())
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