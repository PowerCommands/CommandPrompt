using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.Managers;

namespace PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.Commands;

[CommandDesign(description: "Encrypt or decrypt using PGP", options: ["encrypt", "decrypt"], examples: ["pgp --encrypt", "pgp --decrypt"])]
public class PgpCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var path = input.GetFullPath();
        if (!File.Exists(path)) return Nok("File not found.");

        var passport = new Passport(
            PublicKeyFilePath: Configuration.Core.Modules.Pgp.PublicKeyFilePath,
            PrivateKeyFilePath: Configuration.Core.Modules.Pgp.PrivateKeyFilePath,
            PassPhrase: DialogService.GetSecret("Enter passphrase")
        );

        var manager = new PgpEncryptionManager(passport.PublicKeyFilePath, passport.PrivateKeyFilePath, passport.PassPhrase);

        if (input.HasOption("encrypt"))
        {
            var content = File.ReadAllText(path);
            var encrypted = manager.Encrypt(content);
            File.WriteAllText(path + ".pgp", encrypted);
            Writer.WriteSuccessLine($"Encrypted to: {path}.pgp");
            return Ok();
        }

        if (input.HasOption("decrypt"))
        {
            var encrypted = File.ReadAllText(path);
            var decrypted = manager.Decrypt(encrypted);
            File.WriteAllText(path + ".decrypted", decrypted);
            Writer.WriteSuccessLine($"Decrypted to: {path}.decrypted");
            return Ok();
        }
        return Nok("Use either --encrypt or --decrypt.");
    }
}