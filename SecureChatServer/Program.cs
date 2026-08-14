using System.Net;
using System.Net.Sockets;
using SecureChatShared;

Console.Title = "Secure Chat Server";

var listener = new TcpListener(IPAddress.Any, 8080);
listener.Start();

Console.WriteLine("[SERVER] Waiting for connections on port 8080...");

using TcpClient client = await listener.AcceptTcpClientAsync();
using NetworkStream stream = client.GetStream();

Console.WriteLine("[SERVER] Client connected.");

Console.WriteLine("\n=== DIFFIE-HELLMAN HANDSHAKE ===");

int b = DiffieHellmanService.GeneratePrivateNumber();
int B = DiffieHellmanService.ComputePublicValue(b);

string? receivedA = await PacketIO.ReceiveStringAsync(stream);

if (receivedA == null)
{
    Console.WriteLine("[SERVER] Public value A was not received.");
    return;
}

int A = int.Parse(receivedA);

await PacketIO.SendStringAsync(stream, B.ToString());

int sharedSecret = DiffieHellmanService.ComputeSharedSecret(A, b);
byte[] aesKey = DiffieHellmanService.DeriveAesKey(sharedSecret);

Console.WriteLine($"[SERVER] p = {DiffieHellmanService.P}, g = {DiffieHellmanService.G}");
Console.WriteLine($"[SERVER] Private secret b = {b}");
Console.WriteLine($"[SERVER] Received public value A = {A}");
Console.WriteLine($"[SERVER] Sent public value B = {B}");
Console.WriteLine($"[SERVER] Shared secret S = {sharedSecret}");
Console.WriteLine($"[SERVER] AES key SHA256(S) = {Convert.ToBase64String(aesKey)}");

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

            Console.WriteLine("\n[SERVER] Packet received:");
            Console.WriteLine($"IV: {packet.IV}");
            Console.WriteLine($"CipherText: {packet.CipherText}");
            Console.WriteLine($"Hash: {packet.Hash}");

            string decrypted = CryptoService.DecryptMessage(packet, aesKey);

            Console.WriteLine($"[SERVER] Decrypted message: {decrypted}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVER] Error: {ex.Message}");
            break;
        }
    }
});

while (true)
{
    Console.Write("[SERVER] Enter message: ");
    string? message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message))
        continue;

    SecurePacket packet = CryptoService.EncryptMessage(message, aesKey);

    Console.WriteLine("[SERVER] Encrypted packet sent:");
    Console.WriteLine($"IV: {packet.IV}");
    Console.WriteLine($"CipherText: {packet.CipherText}");
    Console.WriteLine($"Hash: {packet.Hash}");

    await PacketIO.SendPacketAsync(stream, packet);
}