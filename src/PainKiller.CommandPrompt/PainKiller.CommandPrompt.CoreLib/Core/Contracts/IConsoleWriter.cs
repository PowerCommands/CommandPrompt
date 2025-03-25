namespace PainKiller.CommandPrompt.CoreLib.Core.Contracts;
public interface IConsoleWriter
{
    void Write(string text, bool writeLog = true, ConsoleColor consoleColor = ConsoleColor.Black ,string scope = "");
    void WriteLine(string text, bool writeLog = true, ConsoleColor consoleColor = ConsoleColor.Black, string scope = "");
    void WriteSuccessLine(string text, bool writeLog = true, string scope = "");
    void WriteWarning(string text, string scope = "");
    void WriteError(string text, string scope = "");
    void WriteCritical(string text, string scope = "");
    void WriteHeaderLine(string text, bool writeLog = true, string scope = "");
    void WriteUrl(string text, bool writeLog = true, string scope = "");
    void WritePrompt(string prompt);
    void Clear();
}