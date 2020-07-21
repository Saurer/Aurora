using MessagePack;

namespace AuroraCore.Networking {
    [MessagePackObject]
    public class HandshakePacket : Packet {
        [Key(0)]
        public string VerMagic { get; private set; } = Const.Version;

        [Key(1)]
        public int Position { get; private set; }

        public HandshakePacket() { }

        public HandshakePacket(int position) {
            Position = position;
        }
    }
}