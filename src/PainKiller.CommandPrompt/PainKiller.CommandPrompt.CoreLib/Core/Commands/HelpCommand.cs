﻿using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
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

        InteractiveFilter<HelpEntry>.Run(entries, EntryFilter, DisplayTable);
        return Ok();
    }
    private bool EntryFilter(HelpEntry entry, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter)) return true;
        return entry.Identifier.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }
    private void DisplayTable(IEnumerable<HelpEntry> entries)
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

        foreach (var entry in list)
        {
            table.AddRow(
                new Markup($"[green]{Markup.Escape(entry.Identifier)}[/]"),
                new Markup(Markup.Escape(entry.Description))
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
}

