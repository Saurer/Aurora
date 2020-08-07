using AuroraCore.Storage;
using MessagePack;

namespace AuroraCore.Networking {
    [MessagePackObject]
    public class EventPacket : Packet {
        public IEventData Value { get; private set; }

        public EventPacket(IEventData value) {
            Value = value;
        }
    }
}