namespace SecureChatShared;

public class SecurePacket
{
    public string IV { get; set; } = "";
    public string CipherText { get; set; } = "";
    public string Hash { get; set; } = "";
}