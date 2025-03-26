using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;

namespace PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;

public abstract class ConsoleCommandBase<TConfig>(string identifier) : IConsoleCommand
{
    protected IConsoleWriter Console { get; } = new SpectreConsoleWriter();
    public string Identifier { get; } = identifier;
    public TConfig Configuration { get; private set; } = default!;
    private void SetConfiguration(TConfig config) => Configuration = config;
    public abstract RunResult Run(ICommandLineInput input);
    protected RunResult Ok(string message = "") => new RunResult(Identifier, true, message);
    protected RunResult Nok(string message = "") => new RunResult(Identifier, false, message);
}
