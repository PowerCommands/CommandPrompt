using PainKiller.CommandPrompt.CoreLib.Configuration.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Configuration;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Services;

namespace PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;

public static class SecretExtensions
{
    public static void CreateSecret(this SecurityConfiguration configuration, string secretName)
    {
        var secretToken = DialogService.GetSecret("secret");
        var secret = new SecretItemConfiguration { Name = secretName };
        var val = SecretService.Service.SetSecret(secretName, secretToken, secret.Options, EncryptionService.Service.EncryptString);

        configuration.Secrets ??= new();
        configuration.Secrets.Add(secret);
        ConfigurationService.Service.SaveChanges(configuration);
        Console.WriteLine();
        ConsoleService.Writer.WriteHeaderLine("New secret created and stored in configuration file");
        ConsoleService.Writer.WriteDescription( secretName, val);
    }
    public static void AddSecretToConfig(this ApplicationConfiguration configuration, string secretName)
    {
        var secret = new SecretItemConfiguration { Name = secretName };
        configuration.Core.Modules.Security.Secrets ??= new();
        configuration.Core.Modules.Security.Secrets.Add(secret);
        ConfigurationService.Service.SaveChanges(configuration);
    }
}