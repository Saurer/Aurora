using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraCore.Controllers;
using AuroraCore.Storage;

namespace AuroraCore {
    public class EngineBase {
        private IStorageAdapter storage;
        private List<Controller> controllers = new List<Controller>();
        private Dictionary<int, List<Action<IEvent>>> reactions = new Dictionary<int, List<Action<IEvent>>>();

        public IStorageAdapter Storage {
            get {
                return storage;
            }
        }

        public EngineBase(IStorageAdapter storageAdapter) {
            storage = storageAdapter;
        }

        public void AddReaction(int eventID, Action<IEvent> reaction) {
            if (!reactions.ContainsKey(eventID)) {
                reactions[eventID] = new List<Action<IEvent>>();
            }

            reactions[eventID].Add(reaction);
        }

        public async Task<IEnumerable<Action<IEvent>>> GetReactionsFor(IEvent e) {
            var result = new List<Action<IEvent>>();

            foreach (var reaction in reactions) {
#warning TODO: Implement memory cache
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

        public async Task ProcessEvent(IEvent e) {
            var reactions = await GetReactionsFor(e);
            foreach (var handler in reactions) {
                handler(e);
            }

#warning TODO: Catch exceptions
            await Storage.AddEvent(e);
        }
    }
}