using System;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using java.security;

namespace Craft.Net.Server
{
    public class EncryptionKeyRequestPacket : Packet
    {
        public string AuthenticationHash;
        public PublicKey PublicKey;

        public EncryptionKeyRequestPacket()
        {
        }

        public EncryptionKeyRequestPacket(string AuthenticationHash, PublicKey PublicKey)
        {
            this.AuthenticationHash = AuthenticationHash;
            this.PublicKey = PublicKey;
        }

        public override byte PacketID
        {
            get
            {
                return 0xFD;
            }
        }

        public override int TryReadPacket(byte[] Buffer, int Length)
        {
            throw new InvalidOperationException();
        }

        public override void HandlePacket(MinecraftServer Server, ref MinecraftClient Client)
        {
            throw new InvalidOperationException();
        }

        public override void SendPacket(MinecraftServer Server, MinecraftClient Client)
        {
            byte[] verifyToken = new byte[4];
            RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
            csp.GetBytes(verifyToken); // TODO: Encrypt this

            byte[] certificate = PublicKey.getEncoded();

            byte[] buffer = new byte[] { PacketID }
                .Concat(CreateString(AuthenticationHash))
                .Concat(CreateShort((short)certificate.Length))
                .Concat(certificate)
                .Concat(CreateShort((short)verifyToken.Length))
                .Concat(verifyToken).ToArray();
            Client.Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);
        }
    }
}

