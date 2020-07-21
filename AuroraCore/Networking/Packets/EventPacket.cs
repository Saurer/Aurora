using AuroraCore.Storage;
using MessagePack;

namespace AuroraCore.Networking {
    [MessagePackObject]
    public class EventPacket : Packet {
        public IEvent Value { get; private set; }

        public EventPacket(IEvent value) {
            Value = value;
        }
    }
}