using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AuroraCore.Controllers;
using AuroraCore.Networking;
using AuroraCore.Storage;

namespace AuroraCore {
    public class EngineBase {
        private IStorageAdapter storage;
        private List<Controller> controllers = new List<Controller>();
        private Dictionary<int, List<Func<IEvent, Task>>> reactions = new Dictionary<int, List<Func<IEvent, Task>>>();
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

        public void AddReaction(int eventID, Func<IEvent, Task> reaction) {
            if (!reactions.ContainsKey(eventID)) {
                reactions[eventID] = new List<Func<IEvent, Task>>();
            }

            reactions[eventID].Add(reaction);
        }

        public async Task<IEnumerable<Func<IEvent, Task>>> GetReactionsFor(IEvent e) {
            var result = new List<Func<IEvent, Task>>();

            foreach (var reaction in reactions) {
                // TODO: It is possible to cache 
                var isAncestor = await Storage.IsEventAncestor(reaction.Key, e.ValueID);
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
                AddReaction(reaction.EventID, async (e) => {
                    await (Task)reaction.Method.Invoke(controller, new[] {
                        e
                    });
                });
            }
            controllers.Add(controller);
        }

        public void AddNetworkAdapter(INetworkAdapter adapter) {
            netMon.AddNetworkAdapter(adapter);
        }

        public async Task ProcessEvent(IEvent e) {
            if (netMon.State == NetworkState.Sync) {
                throw new Exception("Engine is syncing");
            }

            var reactions = await GetReactionsFor(e);
            foreach (var handler in reactions) {
                await handler(e);
            }

#warning TODO: Catch exceptions
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

        public async Task Restore(IEnumerable<IEvent> events) {
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