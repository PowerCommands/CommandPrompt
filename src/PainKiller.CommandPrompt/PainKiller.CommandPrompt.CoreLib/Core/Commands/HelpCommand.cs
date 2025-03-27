using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using Spectre.Console;

namespace PainKiller.CommandPrompt.CoreLib.Core.Commands;

[CommandDesign("Displays a list of available commands")]
public class HelpCommand(string identity) : ConsoleCommandBase<ApplicationConfiguration>(identity)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var table = new Table().Centered().RoundedBorder().AddColumn("[bold yellow]Command[/]").AddColumn("[bold]Description[/]");
        foreach (var meta in MetadataRegistryService.ReaderInstance.All.Values.OrderBy(m => m.Identifier)) table.AddRow($"[green]{meta.Identifier}[/]", meta.Description);
        AnsiConsole.Write(table);
        return Ok();
    }
}
