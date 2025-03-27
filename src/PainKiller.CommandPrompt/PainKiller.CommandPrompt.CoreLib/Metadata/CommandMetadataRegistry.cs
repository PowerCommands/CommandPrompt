using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Metadata.Contracts;
using PainKiller.CommandPrompt.CoreLib.Metadata.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Extensions;

namespace PainKiller.CommandPrompt.CoreLib.Metadata;
public class MetadataRegistryService : IMetadataRegistry
{
    private static readonly Lazy<IMetadataRegistry> Instance = new(() => new MetadataRegistryService());
    private MetadataRegistryService() { }
    internal static IMetadataRegistry WritableInstance => Instance.Value;
    public static IMetadataRegistryReader ReaderInstance => Instance.Value;

    private readonly Dictionary<string, CommandMetadata> _map = new();
    public void Register(IConsoleCommand command)
    {
        var metadata = command.GetMetadata();
        if (metadata != null)
        {
            _map[command.Identifier] = metadata;
        }
    }
    public CommandMetadata? Get(string identifier) => _map.GetValueOrDefault(identifier);
    public IReadOnlyDictionary<string, CommandMetadata> All => _map;
}
