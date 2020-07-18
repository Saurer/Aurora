using System;
using AuroraCore.Networking;
using MessagePack;

namespace Aurora.Networking.Tcp {
    public static class Serializer {
        public static byte[] Serialize<T>(T packet) where T : Packet {
            return MessagePackSerializer.Serialize<T>(packet);
        }

        public static T Deserialize<T>(byte[] data) where T : Packet {
            return MessagePackSerializer.Deserialize<T>(data);
        }

        public static Packet Deserialize(byte[] data, Type type) {
            return (Packet)MessagePackSerializer.Deserialize(type, data);
        }
    }
}