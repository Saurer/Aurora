using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using AuroraCore.Networking;

namespace Aurora.Networking.Tcp {
    public class TcpNetworkAdapter : INetworkAdapter {
        private TcpListener listener;
        private Dictionary<Connection, Task> connections = new Dictionary<Connection, Task>();

        public event EventHandler<Packet> OnPacket;

        public TcpNetworkAdapter(IPEndPoint bindEndpoint) {
            listener = new TcpListener(bindEndpoint);
        }

        public async Task Listen() {
            listener.Start();
            while (true) {
                TcpClient client = await listener.AcceptTcpClientAsync();
                var connection = new Connection(this, client);
                var listenTask = connection.Listen();
                connections.Add(connection, listenTask);
                Console.WriteLine($"Accepted: {client.Client.RemoteEndPoint}");
            }
        }

        public async Task EmitEvent(EventPacket packet, int position) {
            foreach (var connection in connections) {
                // if (connection.Key.Position != position) {
                //     continue;
                // }

                try {
                    await connection.Key.Send(packet);
                }
                catch (Exception) {
                    Disconnect(connection.Key);
                }
            }
        }

        internal void Disconnect(Connection connection) {
            if (connections.TryGetValue(connection, out var task)) {
                connections.Remove(connection);
                task.Dispose();
            }
        }

        public void Dispose() {
            listener.Stop();
        }

        public async Task Connect(IPEndPoint endpoint) {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 20854);
            var con = new Connection(this, client);
            await con.Send(new HandshakePacket(0));
        }
    }
}