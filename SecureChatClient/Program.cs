using System.Net.Sockets;
using SecureChatShared;

Console.Title = "Secure Chat Client";

using TcpClient client = new TcpClient();

Console.WriteLine("[CLIENT] Connecting to server...");
await client.ConnectAsync("127.0.0.1", 8080);

using NetworkStream stream = client.GetStream();

Console.WriteLine("[CLIENT] Connected to server.");

Console.WriteLine("\n=== DIFFIE-HELLMAN HANDSHAKE ===");

int a = DiffieHellmanService.GeneratePrivateNumber();
int A = DiffieHellmanService.ComputePublicValue(a);

await PacketIO.SendStringAsync(stream, A.ToString());

string? receivedB = await PacketIO.ReceiveStringAsync(stream);

if (receivedB == null)
{
    Console.WriteLine("[CLIENT] Public value B was not received.");
    return;
}

int B = int.Parse(receivedB);

int sharedSecret = DiffieHellmanService.ComputeSharedSecret(B, a);
byte[] aesKey = DiffieHellmanService.DeriveAesKey(sharedSecret);

Console.WriteLine($"[CLIENT] p = {DiffieHellmanService.P}, g = {DiffieHellmanService.G}");
Console.WriteLine($"[CLIENT] Private secret a = {a}");
Console.WriteLine($"[CLIENT] Sent public value A = {A}");
Console.WriteLine($"[CLIENT] Received public value B = {B}");
Console.WriteLine($"[CLIENT] Shared secret S = {sharedSecret}");
Console.WriteLine($"[CLIENT] AES key SHA256(S) = {Convert.ToBase64String(aesKey)}");

Console.WriteLine("\n=== SECURE CHAT ===");

_ = Task.Run(async () =>
{
    while (true)
    {
        try
        {
            SecurePacket? packet = await PacketIO.ReceivePacketAsync(stream);

            if (packet == null)
                break;

            Console.WriteLine("\n[CLIENT] Packet received:");
            Console.WriteLine($"IV: {packet.IV}");
            Console.WriteLine($"CipherText: {packet.CipherText}");
            Console.WriteLine($"Hash: {packet.Hash}");

            string decrypted = CryptoService.DecryptMessage(packet, aesKey);

            Console.WriteLine($"[CLIENT] Decrypted message: {decrypted}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CLIENT] Error: {ex.Message}");
            break;
        }
    }
});

while (true)
{
    Console.Write("[CLIENT] Enter message: ");
    string? message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message))
        continue;

    SecurePacket packet = CryptoService.EncryptMessage(message, aesKey);

    Console.WriteLine("[CLIENT] Encrypted packet sent:");
    Console.WriteLine($"IV: {packet.IV}");
    Console.WriteLine($"CipherText: {packet.CipherText}");
    Console.WriteLine($"Hash: {packet.Hash}");

    await PacketIO.SendPacketAsync(stream, packet);
}