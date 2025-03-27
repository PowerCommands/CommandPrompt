using System.Reflection;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Metadata;
using PainKiller.CommandPrompt.CoreLib.Metadata.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Core.Services;

public static class CommandDiscoveryService
{
    public static List<IConsoleCommand> DiscoverCommands(object? configuration = null)
    {
        IMetadataRegistry metadataRegistry = MetadataRegistryService.WritableInstance;
        var commands = new List<IConsoleCommand>();

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IConsoleCommand).IsAssignableFrom(t) && !t.IsAbstract && t.Name.EndsWith("Command"));

        foreach (var type in types)
        {
            var identifier = type.Name[..^7].ToLowerInvariant();
            var ctor = type.GetConstructor([typeof(string)]);
            if (ctor is null) continue;

            var instance = ctor.Invoke([identifier]);

            if (configuration != null)
            {
                var baseType = type.BaseType;
                var setMethod = baseType?.GetMethod("SetConfiguration", BindingFlags.Instance | BindingFlags.NonPublic);
                var configProperty = baseType?.GetProperty("Configuration", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (setMethod != null && configProperty != null && configProperty.PropertyType.IsInstanceOfType(configuration))
                {
                    setMethod.Invoke(instance, [configuration]);
                }
            }
            var command = (IConsoleCommand)instance;
            if (commands.Any(c => c.Identifier == command.Identifier)) continue;
            commands.Add(command);
            metadataRegistry.Register(command);
        }
        return commands;
    }
}
