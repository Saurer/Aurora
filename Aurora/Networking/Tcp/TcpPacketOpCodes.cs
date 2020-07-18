using System;
using System.Collections.Generic;
using AuroraCore.Networking;

namespace Aurora.Networking.Tcp {
    public class TcpPacketOpCodes {
        private Dictionary<uint, Type> opcodes = new Dictionary<uint, Type>();
        private Dictionary<Type, uint> packets = new Dictionary<Type, uint>();

        private TcpPacketOpCodes() {
            RegisterPacket<HandshakePacket>(1);
        }

        private void RegisterPacket<T>(uint opcode) where T : Packet {
            opcodes.Add(opcode, typeof(T));
            packets.Add(typeof(T), opcode);
        }

        private static TcpPacketOpCodes instance = new TcpPacketOpCodes();

        public static Type GetType(uint opcode) {
            return instance.opcodes[opcode];
        }

        public static uint GetOpCode<T>() where T : Packet {
            return instance.packets[typeof(T)];
        }
    }
}