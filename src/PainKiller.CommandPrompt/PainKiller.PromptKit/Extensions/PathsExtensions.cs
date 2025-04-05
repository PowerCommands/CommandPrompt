namespace PainKiller.PromptKit.Extensions;

public static class PathsExtensions
{
    public static DirectoryInfo GetRoot(this DirectoryInfo directory)
    {
        if (directory.FullName == AppContext.BaseDirectory) return Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent!;
        if (directory.Name == "PainKiller.CommandPrompt.CoreLib") return directory.Parent!;
        if (directory.GetFiles("program.cs").Length == 1) return directory.Parent!;
        if (directory.GetFiles("CommandPromptConfiguration.yaml").Length == 1) return directory.Parent!;
        return directory.Parent?.Parent!;
    }
}