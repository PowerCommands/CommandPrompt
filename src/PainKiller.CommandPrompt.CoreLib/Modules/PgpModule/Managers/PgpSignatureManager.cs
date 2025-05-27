using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.Managers;

public class PgpSignatureManager(string publicKeyFilePath, string privateKeyFilePath, string passphrase) : PgpManagerBase(publicKeyFilePath, privateKeyFilePath, passphrase)
{
    public string Sign(string message)
    {
        var messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        using MemoryStream signatureStream = new MemoryStream();
        var pgpPrivateKey = GetPgpPrivateKey();

        if (SecretKey == null) return Convert.ToBase64String(signatureStream.ToArray());

        var signatureGenerator = new PgpSignatureGenerator(SecretKey.PublicKey.Algorithm, HashAlgorithmTag.Sha256);
        signatureGenerator.InitSign(PgpSignature.BinaryDocument, pgpPrivateKey);

        var bOut = new BcpgOutputStream(signatureStream);

        signatureGenerator.Update(messageBytes, 0, messageBytes.Length);
        var signature = signatureGenerator.Generate();
        signature.Encode(bOut);
        return Convert.ToBase64String(signatureStream.ToArray());
    }

    public bool Verify(string message, string signature)
    {
        var messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        var signatureBytes = Convert.FromBase64String(signature);

        using Stream sigStream = new MemoryStream(signatureBytes);
        using Stream messageStream = new MemoryStream(messageBytes);
        var pgpFactory = new PgpObjectFactory(PgpUtilities.GetDecoderStream(sigStream));
        var signatureList = (PgpSignatureList)pgpFactory.NextPgpObject();
        var pgpSignature = signatureList[0];

        var pubKey = GetPgpPubKey();
        pgpSignature.InitVerify(pubKey);

        using (Stream messageInputStream = new MemoryStream(messageBytes))
        {
            int ch;
            while ((ch = messageInputStream.ReadByte()) >= 0)
            {
                pgpSignature.Update((byte)ch);
            }
        }
        return pgpSignature.Verify();
    }
}