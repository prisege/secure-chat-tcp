using System.Security.Cryptography;
using System.Text;

namespace SecureChatShared;

public static class CryptoService
{
    public static SecurePacket EncryptMessage(string plainText, byte[] aesKey)
    {
        using Aes aes = Aes.Create();

        aes.Key = aesKey;
        aes.GenerateIV();

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] hashBytes = SHA256.HashData(cipherBytes);

        return new SecurePacket
        {
            IV = Convert.ToBase64String(aes.IV),
            CipherText = Convert.ToBase64String(cipherBytes),
            Hash = Convert.ToBase64String(hashBytes)
        };
    }

    public static string DecryptMessage(SecurePacket packet, byte[] aesKey)
    {
        byte[] iv = Convert.FromBase64String(packet.IV);
        byte[] cipherBytes = Convert.FromBase64String(packet.CipherText);
        byte[] receivedHash = Convert.FromBase64String(packet.Hash);

        byte[] computedHash = SHA256.HashData(cipherBytes);

        if (!receivedHash.SequenceEqual(computedHash))
        {
            throw new Exception("Eroare de integritate: hash-ul nu corespunde!");
        }

        using Aes aes = Aes.Create();

        aes.Key = aesKey;
        aes.IV = iv;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}