using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Configuration;

namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
public class ApplicationConfiguration
{
    public CoreConfiguration Core { get; set; } = new();
    public LogConfiguration Log { get; set; } = new(fileName: "commandprompt.log", filePath: "logs", restrictedToMinimumLevel: "Information", rollingIntervall: "Day");
    public List<string> Suggestions { get; set; } = [];
    public string RoamingDirectoryName { get; set; } = $"{nameof(CommandPrompt)}";
    public SecurityConfiguration Security { get; set; } = new();
    public static string CoreApplicationDataPath
    {
        get
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(CommandPrompt));
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            return directory;
        }
    }
}