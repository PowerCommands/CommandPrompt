using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ReadLine.Managers;

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

        if (arg == "\\")
        {
            path = Directory.GetDirectoryRoot(path);
        }
        else if (arg == "..")
        {
            path = Path.GetDirectoryName(path) ?? path;
        }
        else if (!string.IsNullOrWhiteSpace(arg))
        {
            path = Path.Combine(path, arg);
        }
        if (lowerArgs.Contains("roaming"))
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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
            Console.WriteLine($"[cd] Path not found: {path}");
        }

        ShowDirectories();
        return Ok();
    }
    private void ShowDirectories(string filter = "")
    {
        var dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
        Console.WriteLine($"Current directory: {dirInfo.FullName}");

        var fileSuggestions = new List<string>();
        var dirSuggestions = new List<string>();

        var userFilter = filter.ToLowerInvariant();

        var files = dirInfo.GetFiles();
        var directories = dirInfo.GetDirectories();

        Console.WriteLine($"Files: {files.Length}");
        Console.WriteLine($"Directories: {directories.Length}");

        foreach (var directoryInfo in directories.Where(d => d.Name.ToLower().Contains(userFilter)))
        {
            Console.WriteLine($"<DIR> {directoryInfo.Name}");
            dirSuggestions.Add(directoryInfo.Name);
        }

        foreach (var fileInfo in files.Where(f => f.Name.ToLower().Contains(userFilter)))
        {
            Console.WriteLine($"      {fileInfo.Name}");
            fileSuggestions.Add(fileInfo.Name);
        }
        SuggestionProviderManager.AppendContextBoundSuggestions(Identifier, dirSuggestions.Concat(fileSuggestions).ToArray());
    }
}