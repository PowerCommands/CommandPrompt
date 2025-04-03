using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Services;
using Spectre.Console;

namespace PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Commands;

[CommandDesign(description:"Start the InfoPanel, this command just shows how you can implement the InfoPanel, let one Command class run RegisterContent in the OnInitialized method.",
                   options: ["stop"],
                  examples: ["//Update the InfoPanel","infopanel","//Stop the InfoPanel refresh","infopanel --stop"])]
public class InfoPanelCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    public override void OnInitialized() => InfoPanelService.Instance.RegisterContent(new SpectreInfoPanel(new DefaultInfoPanelContent(), Color.Magenta1, Color.BlueViolet));
    public override RunResult Run(ICommandLineInput input)
    {
        if(input.HasOption("stop")) InfoPanelService.Instance.Stop();
        else InfoPanelService.Instance.Update(); 
        return Ok();
    }
}