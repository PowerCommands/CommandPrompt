﻿using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Core.Runtime;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Metadata;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using Spectre.Console;

namespace PainKiller.CommandPrompt.CoreLib.Core.Commands;

[CommandDesign("Displays a list of available commands")]
public class HelpCommand(string identity) : ConsoleCommandBase<ApplicationConfiguration>(identity)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var entries = MetadataRegistryService.ReaderInstance.All.Values
            .OrderBy(m => m.Identifier)
            .Select(m => new HelpEntry
            {
                Identifier = m.Identifier,
                Description = m.Description
            })
            .ToList();

        InteractiveFilter<HelpEntry>.Run(entries, EntryFilter, DisplayTable, OnSelected);
        return Ok();
    }
    private bool EntryFilter(HelpEntry entry, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter)) return true;
        return entry.Identifier.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }
    private void DisplayTable(IEnumerable<HelpEntry> entries, int selectedIndex)
    {
        var list = entries.ToList();

        if (list.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No matching commands found.[/]");
            return;
        }

        var table = new Table()
            .Expand()
            .RoundedBorder()
            .AddColumn(new TableColumn("[bold yellow]Command[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Description[/]").LeftAligned());

        for (int i = 0; i < list.Count; i++)
        {
            var entry = list[i];
            var isSelected = i == selectedIndex;

            var prefix = isSelected ? "[bold cyan]>[/] " : "  ";
            var command = isSelected
                ? $"[bold cyan]{Markup.Escape(entry.Identifier)}[/]"
                : $"[green]{Markup.Escape(entry.Identifier)}[/]";

            var description = isSelected
                ? $"[italic]{Markup.Escape(entry.Description)}[/]"
                : Markup.Escape(entry.Description);

            table.AddRow(
                new Markup(prefix + command),
                new Markup(description)
            );
        }

        table.AddEmptyRow();
        table.AddRow(
            new Markup("[bold]Total[/]"),
            new Markup($"{list.Count} command(s)")
        );

        AnsiConsole.Write(table);
    }
    private class HelpEntry
    {
        public string Identifier { get; init; } = "";
        public string Description { get; init; } = "";
    }
    private void OnSelected(HelpEntry entry)
    {
        Console.Clear();
        if (!CommandDiscoveryService.TryGetCommand("describe", out var describeCommand))
        {
            AnsiConsole.MarkupLine($"[red]Could not find 'describe' command.[/]");
            return;
        }
        var input = new CommandLineInput($"describe {entry.Identifier}", "describe", [entry.Identifier], [], new Dictionary<string, string>());
        var executor = new CommandExecutor();
        executor.Execute(describeCommand, input);
    }

}

