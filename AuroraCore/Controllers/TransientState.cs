using System;
using System.Collections.Generic;
using AuroraCore.Storage;

namespace AuroraCore.Controllers {
    public class TransientState {
        private Dictionary<int, IEventData> events = new Dictionary<int, IEventData>();
        private Dictionary<int, List<IEventData>> descendants = new Dictionary<int, List<IEventData>>();
        private Dictionary<int, int> ancestors = new Dictionary<int, int>();

        public void Register(IEventData e) {
            if (events.TryGetValue(e.BaseEventID, out var parent) || IsGenesisValue(e)) {
                events[e.ID] = e;

                if (!descendants.ContainsKey(e.BaseEventID)) {
                    descendants[e.BaseEventID] = new List<IEventData>();
                }

                descendants[e.BaseEventID].Add(e);
                ancestors[e.ID] = parent?.ID ?? e.ID;
            }
            else {
                throw new Exception("Value ID does not exist");
            }
        }

        public IEventData Get(int eventID) {
            return events[eventID];
        }

        public bool IsEventAncestor(int ancestor, int checkValue) {
            var queue = new Queue<int>(new[] { checkValue });
            while (queue.Count > 0) {
                int eventID = queue.Dequeue();

                if (eventID == ancestor) {
                    return true;
                }

                if (ancestors.TryGetValue(eventID, out var newEventID) && eventID != newEventID) {
                    queue.Enqueue(newEventID);
                }
            }

            return false;
        }

        private bool IsGenesisValue(IEventData e) =>
            e.ID == e.ValueID && events.Count == 0;
    }
}