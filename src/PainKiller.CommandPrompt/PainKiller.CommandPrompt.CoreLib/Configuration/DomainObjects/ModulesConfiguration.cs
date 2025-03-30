using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Configuration;
namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
public class ModulesConfiguration
{
    public SecurityConfiguration Security { get; set; } = new();
}