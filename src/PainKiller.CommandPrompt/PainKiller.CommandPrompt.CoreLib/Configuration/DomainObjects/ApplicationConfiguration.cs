using PainKiller.CommandPrompt.CoreLib.Configuration.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;

public class ApplicationConfiguration : ICoreConfiguration
{
    public string Name { get; set; } = "Command Prompt";
    public string Prompt { get; set; } = "pcm>";
    public string DefaultCommand { get; set; } = "";
    public LogConfiguration Log { get; set; } = new(fileName: "commandprompt.log", filePath: "logs", restrictedToMinimumLevel: "Information", rollingIntervall: "Day");
    public List<string> Suggestions { get; set; } = [];
    public string RoamingDirectoryName { get; set; } = $"{nameof(CommandPrompt)}";
}