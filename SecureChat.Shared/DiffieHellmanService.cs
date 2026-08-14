using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SecureChatShared;

public static class DiffieHellmanService
{
    public const int P = 23;
    public const int G = 5;

    public static int GeneratePrivateNumber()
    {
        return RandomNumberGenerator.GetInt32(2, P - 2);
    }

    public static int ComputePublicValue(int privateNumber)
    {
        return (int)BigInteger.ModPow(G, privateNumber, P);
    }

    public static int ComputeSharedSecret(int receivedPublicValue, int privateNumber)
    {
        return (int)BigInteger.ModPow(receivedPublicValue, privateNumber, P);
    }

    public static byte[] DeriveAesKey(int sharedSecret)
    {
        byte[] secretBytes = Encoding.UTF8.GetBytes(sharedSecret.ToString());
        return SHA256.HashData(secretBytes);
    }
}