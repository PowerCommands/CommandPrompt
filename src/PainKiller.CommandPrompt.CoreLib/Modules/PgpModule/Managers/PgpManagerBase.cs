using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;

namespace PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.Managers;
public class PgpManagerBase(string publicKeyFilePath, string privateKeyFilePath, string passphrase)
{
    protected PgpSecretKey? SecretKey;
    protected PgpPublicKey? GetPgpPubKey()
    {
        using Stream keyFileStream = File.OpenRead(publicKeyFilePath);
        using ArmoredInputStream armoredInputStream = new(keyFileStream);
        try
        {
            var keyData = Streams.ReadAll(armoredInputStream);
            
            using Stream keyDataStream = new MemoryStream(keyData);
            PgpPublicKeyRingBundle publicKeyRingBundle = new(keyDataStream);

            var publicKeyRing = publicKeyRingBundle.GetKeyRings().First();

            if (publicKeyRing != null)
            {
                return publicKeyRing.GetPublicKeys().First();
            }
            throw new InvalidOperationException("No public key ring found in the specified key file.");
        }
        catch (PgpException ex)
        {
            Console.WriteLine($"Error reading the key file: {ex.Message}");
            return null;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error reading the key file: {ex.Message}");
            return null;
        }
    }
    protected PgpPrivateKey? GetPgpPrivateKey()
    {
        try
        {
            PgpSecretKeyRing secretKeyRing;
            using (var keyFileStream = new FileStream(privateKeyFilePath, FileMode.Open))
            {
                secretKeyRing = new PgpSecretKeyRing(PgpUtilities.GetDecoderStream(keyFileStream));
            }
            SecretKey = secretKeyRing.GetSecretKey();
            var privateKey = SecretKey.ExtractPrivateKey($"{passphrase}".ToCharArray());

            return privateKey;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading PGP private key from file: {ex.Message}");
            return null;
        }
    }
}