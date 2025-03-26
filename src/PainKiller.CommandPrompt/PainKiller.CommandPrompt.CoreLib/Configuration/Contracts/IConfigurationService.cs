using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;

namespace PainKiller.CommandPrompt.CoreLib.Configuration.Contracts;
public interface IConfigurationService
{
    YamlContainer<T> Get<T>(string inputFileName = "") where T : new();
    string SaveChanges<T>(T configuration, string inputFileName = "") where T : new();
}