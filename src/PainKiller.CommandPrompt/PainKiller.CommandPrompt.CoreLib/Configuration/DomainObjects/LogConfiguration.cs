namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;

public class LogConfiguration
{
    public string FileName { get; set; } = "powercommands.log";
    public string FilePath { get; set; } = "logs";
    public string RollingIntervall { get; set; } = "Day";
    public string RestrictedToMinimumLevel { get; set; } = "Information";
}