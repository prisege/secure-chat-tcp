using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SecureChatShared;

public static class PacketIO
{
    public static async Task SendPacketAsync(NetworkStream stream, SecurePacket packet)
    {
        string json = JsonSerializer.Serialize(packet);
        byte[] data = Encoding.UTF8.GetBytes(json + "\n");

        await stream.WriteAsync(data, 0, data.Length);
    }

    public static async Task<SecurePacket?> ReceivePacketAsync(NetworkStream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);

        string? json = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<SecurePacket>(json);
    }

    public static async Task SendStringAsync(NetworkStream stream, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        await stream.WriteAsync(data, 0, data.Length);
    }

    public static async Task<string?> ReceiveStringAsync(NetworkStream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadLineAsync();
    }
}