using PainKiller.CommandPrompt.CoreLib.Modules.ChatGptModule.Configuration;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Configuration;
using PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Configuration;
using PainKiller.CommandPrompt.CoreLib.Modules.OllamaModule.Configuration;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Configuration;
using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Configuration;

namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
public class ModulesConfiguration
{
    public SecurityConfiguration Security { get; set; } = new();
    public StorageConfiguration Storage { get; set; } = new();
    public OllamaConfiguration Ollama { get; set; } = new();
    public ChatGptConfiguration ChatGpt { get; set; } = new();
    public InfoPanelConfiguration InfoPanel { get; set; } = new();
    public DatabaseConfig DbStorage { get; set; } = new();

}