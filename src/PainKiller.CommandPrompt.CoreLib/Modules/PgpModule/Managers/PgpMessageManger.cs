using System.Text;
using PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.DomainObjects;

namespace PainKiller.CommandPrompt.CoreLib.Modules.PgpModule.Managers;

public static class PgpMessageManger
{
    public static string CreateMessage(string message, string signature, string hashAlgorithm = "SHA256")
    {
        var messageBuilder = new StringBuilder();

        var signedText = string.IsNullOrEmpty(signature) ? "" : " SIGNED";
        var unsignedText = string.IsNullOrEmpty(signature) ? "-----END PGP MESSAGE-----" : "";

        messageBuilder.AppendLine($"-----BEGIN PGP{signedText} MESSAGE-----");
        if (!string.IsNullOrEmpty(signature))
        {
            messageBuilder.AppendLine($"Hash: {hashAlgorithm}");
            messageBuilder.AppendLine();
        }
        messageBuilder.AppendLine(message);
        if (string.IsNullOrEmpty(signature))
        {
            messageBuilder.AppendLine(unsignedText);
            return messageBuilder.ToString();
        }
        messageBuilder.AppendLine();
        messageBuilder.AppendLine("-----BEGIN PGP SIGNATURE-----");
        messageBuilder.AppendLine(signature);
        messageBuilder.AppendLine("-----END PGP SIGNATURE-----");
        return messageBuilder.ToString().Trim();
    }
    public static Message ExtractMessage(string body)
    {
        var retVal = new Message();
        var sections = body.Split("-----BEGIN");
        if (body.Length == 0) return retVal;
        var messageRows = sections[1].Split('\n');
        retVal.Hash = messageRows[1].Replace("Hash: ", "").Trim();
        
        var contentBuilder = new StringBuilder();
        var rowCount = 0;
        foreach (var row in messageRows)
        {
            if (rowCount<3)
            {
                rowCount++;
                continue;
            }
            if (row.StartsWith("-----END PGP ")) continue;
            contentBuilder.AppendLine(row);
        }
        retVal.Content = contentBuilder.ToString().Trim();
        if (!body.StartsWith("-----BEGIN PGP SIGNED MESSAGE-----")) return retVal;
        retVal.HasSignature = true;
        var signatureBuilder = new StringBuilder();
        var signatureRows = sections.Last().Split('\n');
        var firstRow = true;
        foreach (var row in signatureRows)
        {
            if (firstRow)
            {
                firstRow = false;
                continue;
            }
            if(row.StartsWith("-----END PGP SIGNATURE-----")) continue;
            signatureBuilder.AppendLine(row);
        }
        retVal.Signature = signatureBuilder.ToString().Trim();
        return retVal;
    }
}