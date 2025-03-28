namespace PainKiller.CommandPrompt.CoreLib.Configuration.Contracts;

public interface ICoreConfiguration
{
    public string Name { get; set; }
    public string Prompt { get; set; }
    public string DefaultCommand { get; set; }
}