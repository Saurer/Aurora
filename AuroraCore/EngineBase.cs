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
using AuroraCore.Controllers;
using AuroraCore.Effects;
using AuroraCore.Networking;
using AuroraCore.Storage;

namespace AuroraCore {
    public class EngineBase {
        private IStorageAdapter storage;
        private List<Controller> controllers = new List<Controller>();
        private Dictionary<int, List<Func<IEventData, Task<Effect>>>> reactions = new Dictionary<int, List<Func<IEventData, Task<Effect>>>>();
        private NetworkManager netMon;

        public IStorageAPI Storage {
            get {
                return storage;
            }
        }

        public int Position { get; private set; } = 0;

        public EngineBase(IStorageAdapter storageAdapter) {
            storage = storageAdapter;
            netMon = new NetworkManager(this);
            AddController<EventController>();
        }

        public void AddReaction(int eventID, Func<IEventData, Task<Effect>> reaction) {
            if (!reactions.ContainsKey(eventID)) {
                reactions[eventID] = new List<Func<IEventData, Task<Effect>>>();
            }

            reactions[eventID].Add(reaction);
        }

        public async Task<IEnumerable<Func<IEventData, Task<Effect>>>> GetReactionsFor(IEventData e) {
            var result = new List<Func<IEventData, Task<Effect>>>();

            foreach (var reaction in reactions) {
                var isAncestor = await storage.IsEventAncestor(reaction.Key, e.ValueID);
                if (isAncestor) {
                    foreach (var item in reaction.Value) {
                        result.Add(item);
                    }
                }
            }

            if (0 == result.Count) {
                throw new Exception("No reactions defined for " + e.ID);
            }

            return result;
        }

        public void AddController<T>() where T : Controller, new() {
            Controller controller = Controller.Instantiate<T>(Storage);
            IEnumerable<ReactionBase> reactions = controller.GetReactions();

            foreach (var reaction in reactions) {
                AddReaction(reaction.EventID, async (e) =>
                    await (Task<Effect>)reaction.Method.Invoke(controller, new[] {
                        e
                    })
                );
            }
            controllers.Add(controller);
        }

        public void AddNetworkAdapter(INetworkAdapter adapter) {
            netMon.AddNetworkAdapter(adapter);
        }

        public async Task ProcessEvent(IEventData e) {
            if (netMon.State == NetworkState.Sync) {
                throw new Exception("Engine is syncing");
            }

            var reactions = await GetReactionsFor(e);
            var queue = new Queue<Effect>();

            try {
                foreach (var handler in reactions) {
                    var effect = await handler(e);
                    queue.Enqueue(effect);
                }
            }
            catch (Exception exception) {
                throw new Exception("Error playing event", exception);
            }


            try {
                while (queue.Count > 0) {
                    var effect = queue.Dequeue();
                    await effect.Execute(storage);
                }
            }
            catch (Exception exception) {
                throw new Exception("Fatal error running effect", exception);
            }

            await storage.AddEvent(e);
            Position++;
        }

        public async Task ProcessNetworkEvent(EventPacket packet) {
            var reactions = await GetReactionsFor(packet.Value);
            foreach (var handler in reactions) {
                await handler(packet.Value);
            }

#warning TODO: Catch exceptions
            await storage.AddEvent(packet.Value);
            Position++;
        }

        public async Task Restore(IEnumerable<IEventData> events) {
            await storage.Prune();
            Position = 0;

            foreach (var e in events) {
                await ProcessEvent(e);
            }
        }

        public async Task Connect<T>(IPEndPoint endpoint) where T : INetworkAdapter {
            await netMon.Connect<T>(endpoint);
        }
    }
}