namespace PainKiller.CommandPrompt.CoreLib.Modules.ShellModule;
public class FileChecksum(string fileName)
{
    public string Mde5Hash { get; } = ChecksumManager.CalculateMd5ForFile(fileName);
}