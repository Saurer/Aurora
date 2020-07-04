using System;
using System.Collections.Generic;
using AuroraCore.Controllers;
using AuroraCore.Storage;
using AuroraCore.Types;

namespace AuroraCore {
    public class EngineBase {
        private List<Controller> controllers = new List<Controller>();
        private Dictionary<int, List<Action<IEventData>>> reactions = new Dictionary<int, List<Action<IEventData>>>();

        public TransientState State { get; private set; } = new TransientState();

        public void AddReaction(int eventID, Action<IEventData> reaction) {
            if (!reactions.ContainsKey(eventID)) {
                reactions[eventID] = new List<Action<IEventData>>();
            }

            reactions[eventID].Add(reaction);
        }

        public IEnumerable<Action<IEventData>> GetReactionsFor(IEventData e) {
            int execCount = 0;

            foreach (var reaction in reactions) {
                if (State.IsEventAncestor(reaction.Key, e.ValueID)) {
                    foreach (var item in reaction.Value) {
                        execCount++;
                        yield return item;
                    }
                }
            }

            if (0 == execCount) {
                throw new Exception("No reactions defined for " + e.ID);
            }

            yield break;
        }

        public void AddController<T>() where T : Controller, new() {
            Controller controller = Controller.Instantiate<T>(State);
            IEnumerable<ReactionBase> reactions = controller.GetReactions();

            foreach (var reaction in reactions) {
                AddReaction(reaction.EventID, e => {
                    reaction.Method.Invoke(controller, new[] {
                        e
                    });
                });
            }
            controllers.Add(controller);
        }

        public void AddType<T>(string name) where T : DataType {
            State.Types.Register<T>(name);
        }

        public void ProcessEvent(IEventData e) {
            foreach (var handler in GetReactionsFor(e)) {
                handler(e);
            }
        }
    }
}