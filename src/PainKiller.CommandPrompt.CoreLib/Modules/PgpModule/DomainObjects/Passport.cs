namespace PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.DomainObjects;
public record Passport(string PublicKeyFilePath, string PrivateKeyFilePath, string PassPhrase);