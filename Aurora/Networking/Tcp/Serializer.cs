// Aurora 
// Copyright (C) 2020  Frank Horrigan <https://github.com/saurer>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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