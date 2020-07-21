using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AuroraCore.Networking {
    internal enum NetworkState {
        Listen,
        Handshake,
        Sync,
    }

    internal class NetworkManager {
        private Dictionary<INetworkAdapter, Task> networkAdapters = new Dictionary<INetworkAdapter, Task>();
        private Dictionary<Type, INetworkAdapter> networkAdapterTypes = new Dictionary<Type, INetworkAdapter>();
        private EngineBase engine;
        private HandshakePacket hostHandshake;

        public NetworkState State { get; private set; } = NetworkState.Listen;

        public NetworkManager(EngineBase engine) {
            this.engine = engine;
        }

        public void AddNetworkAdapter(INetworkAdapter adapter) {
            var listenTask = adapter.Listen();
            networkAdapters.Add(adapter, listenTask);
            networkAdapterTypes.Add(adapter.GetType(), adapter);
            adapter.OnPacket += HandleClientPacket;
        }

        public async Task Connect<T>(IPEndPoint endpoint) where T : INetworkAdapter {
            if (networkAdapterTypes.TryGetValue(typeof(T), out var adapter)) {
                await adapter.Connect(endpoint);
                State = NetworkState.Handshake;
                adapter.OnPacket += HandleHostPacket;
            }
            else {
                throw new Exception($"Network adapter is not registered {typeof(T)}");
            }
        }

        private async void HandleHostPacket(object sender, Packet packet) {
            switch (State) {
                case NetworkState.Handshake:
                    if (packet is HandshakePacket handshakePacket) {
                        if (handshakePacket.VerMagic != Const.Version) {
                            throw new Exception("Illegal opertaion");
                        }

                        if (handshakePacket.Position > engine.Position) {
                            State = NetworkState.Sync;
                        }
                        else {
                            State = NetworkState.Listen;
                        }

                        hostHandshake = handshakePacket;
                        return;
                    }
                    else {
                        throw new Exception("Illegal opertaion");
                    }

                case NetworkState.Sync: {
                        if (packet is EventPacket eventPacket) {
                            await engine.ProcessNetworkEvent(eventPacket);

                            if (engine.Position == hostHandshake.Position) {
                                State = NetworkState.Listen;
                            }
                        }
                        else {
                            throw new Exception("Illegal opertaion");
                        }
                        break;
                    }

                case NetworkState.Listen: {
                        if (packet is EventPacket eventPacket) {
                            await engine.ProcessNetworkEvent(eventPacket);
                        }
                        else {
                            throw new Exception("Illegal opertaion");
                        }
                        break;
                    }
            }
        }

        private async void HandleClientPacket(object sender, Packet packet) {
            switch (State) {
                case NetworkState.Listen:
                    if (packet is EventPacket eventPacket) {
                        await engine.ProcessNetworkEvent(eventPacket);
                        return;
                    }
                    else {
                        throw new Exception("Illegal opertaion");
                    }

                default:
                    throw new Exception("Illegal operation");
            }
        }
    }
}