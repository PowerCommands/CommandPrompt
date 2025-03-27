﻿namespace PainKiller.CommandPrompt.CoreLib.Modules;
public interface IShellService
{
    void OpenDirectory(string path);
    void OpenWithDefaultProgram(string path, string workingDirectory = "");
    void Execute(string program, string args = "", string workingDirectory = "", bool waitForExit = false);
}