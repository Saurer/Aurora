using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using AuroraCore.Networking;

namespace Aurora.Networking.Tcp {
    internal class Connection : IDisposable {
        private TcpNetworkAdapter adapter;
        private TcpClient client;
        private bool active = true;

        public Connection(TcpNetworkAdapter adapter, TcpClient client) {
            this.client = client;
            this.adapter = adapter;
        }

        public async Task Listen() {
            try {
                NetworkStream stream = client.GetStream();
                var buffer = new byte[128];

                while (active) {
                    byte[] opcodeBytes = await ReadExactBytes(stream, 2);
                    byte[] lengthBytes = await ReadExactBytes(stream, 4);
                    ushort opcode = BitConverter.ToUInt16(opcodeBytes, 0);
                    uint length = BitConverter.ToUInt32(lengthBytes, 0);
                    byte[] data = await ReadExactBytes(stream, (int)length);
                    // Console.WriteLine("Read packet opcode: {0}, length: {1}, data: {2}", opcode, length, BitConverter.ToString(data));
                    Console.WriteLine("Read packet opcode: {0}, length: {1}", opcode, length);

                    var packetType = TcpPacketOpCodes.GetType(opcode);
                    Packet packet = Serializer.Deserialize(data, packetType);
                    Console.WriteLine($"Recv: {packet.GetType().Name}, Length: {length}");
                }
            }
            catch (Exception) {
                adapter.Disconnect(this);
            }
        }

        public async Task Send<T>(T packet) where T : Packet {
            byte[] data = Serializer.Serialize<T>(packet);
            NetworkStream stream = client.GetStream();
            byte[] length = BitConverter.GetBytes(Convert.ToUInt32(data.Length));
            await stream.WriteAsync(new byte[] { 0x01, 0x00 }, 0, 2);
            await stream.WriteAsync(length, 0, length.Length);
            await stream.WriteAsync(data, 0, data.Length);
        }

        private async Task<byte[]> ReadExactBytes(NetworkStream stream, int count) {
            var buffer = new byte[count];
            int offset = 0;
            while (offset != count) {
                int readBytes = await stream.ReadAsync(buffer, offset, count - offset);
                offset += readBytes;

                if (readBytes == 0) {
                    throw new Exception("Socket closed");
                }
            }
            return buffer;
        }

        public void Dispose() {
            active = false;
            client.Dispose();
        }
    }
}