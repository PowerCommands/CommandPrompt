using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ReadLine.Managers;
using Spectre.Console;

namespace PainKiller.CommandPrompt.CoreLib.Modules.ShellModule;

[CommandDesign(
    description: "Change or view the current working directory",
    options: ["bookmark", "roaming", "startup", "recent", "documents", "programs", "windows", "profile", "templates", "videos", "pictures", "music"],
    arguments: ["Path or navigation command such as .. or \\"],
    examples:
    [
        "//View current working directory", "cd",
        "//Traverse down one directory", "cd ..",
        "//Change working directory", "cd C:\\ProgramData",
        "cd 'My Folder'",
        "cd --documents"
    ]
)]
public class CdCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var path = Environment.CurrentDirectory;
        var arg = input.Arguments.FirstOrDefault();
        var lowerArgs = input.Options.Select(o => o.Key.ToLower()).ToList();

        if (arg == "\\") path = Directory.GetDirectoryRoot(path);
        else if (arg == "..") path = Path.GetDirectoryName(path) ?? path;
        else if (!string.IsNullOrWhiteSpace(arg)) path = Path.Combine(path, arg);

        if (lowerArgs.Contains("roaming"))
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Configuration.RoamingDirectoryName);
        else if (lowerArgs.Contains("startup"))
            path = Path.GetDirectoryName(Environment.ProcessPath) ?? path;
        else if (lowerArgs.Contains("documents"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        else if (lowerArgs.Contains("recent"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
        else if (lowerArgs.Contains("windows"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        else if (lowerArgs.Contains("music"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        else if (lowerArgs.Contains("pictures"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        else if (lowerArgs.Contains("videos"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        else if (lowerArgs.Contains("templates"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
        else if (lowerArgs.Contains("profile"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        else if (lowerArgs.Contains("programs"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

        path = path.Trim();

        if (Directory.Exists(path))
        {
            Environment.CurrentDirectory = Path.GetFullPath(path);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red][cd][/]: Path not found: {Markup.Escape(path)}");
            return Nok($"Path not found: {path}");
        }

        ShowCurrentDirectoryContent();
        return Ok();
    }

    private void ShowCurrentDirectoryContent()
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

        SuggestionProviderManager.AppendContextBoundSuggestions(Identifier, entries.Select(e => e.Name).ToArray());

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

    private class DirEntry
    {
        public string Name { get; init; } = "";
        public string Type { get; init; } = "";
        public string Size { get; init; } = "";
        public string Updated { get; init; } = "";
    }
}
