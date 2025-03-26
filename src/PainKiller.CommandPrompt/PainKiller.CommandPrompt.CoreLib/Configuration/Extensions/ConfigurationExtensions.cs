namespace PainKiller.CommandPrompt.CoreLib.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static string GetSafePathRegardlessHowApplicationStarted(this string fileName, string directory = "") => string.IsNullOrEmpty(directory) ? Path.Combine(AppContext.BaseDirectory, fileName) : Path.Combine(AppContext.BaseDirectory, directory, fileName);
}