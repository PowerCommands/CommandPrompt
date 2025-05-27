using System.Text;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

namespace PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.Managers;

public class PgpEncryptionManager(string publicKey, string privateKey, string passphrase) : PgpManagerBase (publicKey, privateKey, passphrase)
{
    public string Encrypt(string contentToEncrypt) => Encrypt(Encoding.UTF8.GetBytes(contentToEncrypt));
    public string Encrypt(byte[] contentToEncrypt)
    {
        var publicKey = GetPgpPubKey();
        if (publicKey == null) return string.Empty;

        var encrypted = EncryptDataWithPublicKey(publicKey, contentToEncrypt);
        return Convert.ToBase64String(encrypted);
    }
    public string Decrypt(string contentToDecrypt)
    {
        var privateKey = GetPgpPrivateKey();
        if (privateKey == null) return string.Empty;

        var decrypted = DecryptDataWithPrivateKey(privateKey, contentToDecrypt);
        return Encoding.UTF8.GetString(decrypted);
    }
    private byte[] EncryptDataWithPublicKey(PgpPublicKey encryptionKey, byte[] plainText)
    {
        using MemoryStream encryptedStream = new();
        PgpLiteralDataGenerator lData = new();
        
        using (var literalDataStream = lData.Open(encryptedStream, PgpLiteralData.Binary, PgpLiteralData.Console, plainText.Length, DateTime.UtcNow))
            literalDataStream.Write(plainText, 0, plainText.Length);
        
        using MemoryStream resultStream = new();
        var encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true, new SecureRandom());

        
        encryptedDataGenerator.AddMethod(encryptionKey);
        
        using (Stream compressedOut = encryptedDataGenerator.Open(resultStream, encryptedStream.ToArray().Length))
        {
            compressedOut.Write(encryptedStream.ToArray(), 0, encryptedStream.ToArray().Length);
        }
        return resultStream.ToArray();
    }
    private static byte[] DecryptDataWithPrivateKey(PgpPrivateKey privateKey, string encryptedB64String)
    {
        try
        {
            var pgpEncryptedData = Convert.FromBase64String(encryptedB64String);
            PgpObjectFactory pgpFact = new(pgpEncryptedData);
            
            var encList = (PgpEncryptedDataList)pgpFact.NextPgpObject();
            
            PgpPublicKeyEncryptedData? encData = null;
            foreach (var pgpEncryptedData1 in encList.GetEncryptedDataObjects())
            {
                var data = (PgpPublicKeyEncryptedData)pgpEncryptedData1;
                if (data.KeyId == privateKey.KeyId)
                {
                    encData = data;
                    break;
                }
            }

            if (encData == null) throw new PgpException("Provided private key not found in encrypted data.");
            
            var clear = encData.GetDataStream(privateKey);
            
            using var decryptedStream = new MemoryStream();
            Streams.PipeAll(clear, decryptedStream);
            
            if (!encData.Verify()) throw new PgpException("Modification check failed.");
            {
                
                var litFact = new PgpObjectFactory(decryptedStream.ToArray());
                
                var litData = (PgpLiteralData)litFact.NextPgpObject();

                // Read the actual data from the literal data input stream
                using var dataStream = litData.GetInputStream();
                var data = Streams.ReadAll(dataStream);
                return data;
            }
        }
        catch (PgpException ex)
        {
            throw new Exception($"Decryption failed: {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new Exception($"IO exception during decryption: {ex.Message}", ex);
        }
    }
}