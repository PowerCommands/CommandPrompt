using PainKiller.CommandPrompt.CoreLib.Configuration.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;

public class ApplicationConfiguration : ICoreConfiguration
{
    public string Prompt { get; set; } = "pcm>";
    public string DefaultCommand { get; set; } = "";
    public LogConfiguration Log { get; set; } = new LogConfiguration() { FileName = "commandprompt.log", FilePath = "logs", RestrictedToMinimumLevel = "Information", RollingIntervall = "Day" };
}