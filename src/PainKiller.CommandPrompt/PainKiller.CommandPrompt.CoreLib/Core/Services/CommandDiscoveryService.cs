using PainKiller.CommandPrompt.CoreLib.Core.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Core.Services;

public static class CommandDiscoveryService
{
    public static List<IConsoleCommand> DiscoverCommands() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => typeof(IConsoleCommand).IsAssignableFrom(t) && !t.IsAbstract && t.Name.EndsWith("Command")).Select(CreateCommandInstance).Where(c => c != null).Cast<IConsoleCommand>().ToList();
    private static IConsoleCommand? CreateCommandInstance(Type type)
    {
        var identifier = type.Name[..^7].ToLower();
        var ctor = type.GetConstructor([typeof(string)]);
        if (ctor != null) return (IConsoleCommand?)ctor.Invoke([identifier]);
        //TODO: Replace this with logging in the future
        Console.WriteLine($"Skipping {type.Name}: Missing constructor (string)");
        return null;
    }
}