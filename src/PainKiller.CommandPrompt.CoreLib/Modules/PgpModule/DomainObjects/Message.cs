namespace PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.DomainObjects;
public class Message
{
    public string Content { get; set; } = "";
    public string Signature { get; set; } = "";
    public string Hash { get; set; } = "";
    public bool HasSignature { get; set; }
}