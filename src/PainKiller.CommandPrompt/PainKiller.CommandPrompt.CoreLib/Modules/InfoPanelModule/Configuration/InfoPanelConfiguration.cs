﻿namespace PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Configuration;

public class InfoPanelConfiguration
{
    public bool Enabled { get; set; }
    public int Height { get; set; } = 2;
    public int UpdateIntervalSeconds { get; set; } = -1;
}