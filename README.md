# Secure TCP Chat

A client-server chat application that demonstrates encrypted communication over TCP using Diffie-Hellman, AES and SHA-256.

### Features

- TCP client-server communication
- Diffie-Hellman key exchange
- Shared AES key derivation
- AES message encryption and decryption
- Random initialization vector for each message
- SHA-256 integrity verification
- JSON packet serialization
- Base64 encoding for encrypted data
- Bidirectional encrypted messaging

### Technologies

- C#
- .NET 10
- TCP sockets
- Diffie-Hellman
- AES encryption
- SHA-256
- JSON serialization
- Asynchronous programming

### Project Structure

- `SecureChatClient` – connects and sends encrypted messages
- `SecureChatServer` – receives and decrypts messages
- `SecureChat.Shared` – contains the cryptographic and packet logic

### Running the Project

1. Clone the repository:

   `git clone https://github.com/prisege/secure-chat-tcp.git`

2. Open `SecureChat.slnx` in Visual Studio.
3. Make sure the .NET 10 SDK is installed.
4. Start `SecureChatServer`.
5. Start `SecureChatClient` in a second console.
6. Enter messages in the client and server consoles.

The server listens on TCP port `8080`.

> This project is intended for educational purposes. It uses small Diffie-Hellman parameters to clearly demonstrate the key exchange process and should not be used for production communication.