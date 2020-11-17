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