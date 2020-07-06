using System;
using System.Collections.Generic;
using AuroraCore.Events;
using AuroraCore.Storage;
using AuroraCore.Types;

namespace AuroraCore.Controllers {
    public interface ITransientState {
        ITypeManager Types { get; }
        bool TryGetEvent(int id, out IEventData value);
        bool TryGetValue<T>(int id, out T value) where T : IEvent;
        bool PropertyRegistered(int id);
    }

    internal class TransientState : ITransientState {
        private Dictionary<int, IEventData> events = new Dictionary<int, IEventData>();
        private Dictionary<int, List<IEventData>> descendants = new Dictionary<int, List<IEventData>>();
        private Dictionary<int, int> ancestors = new Dictionary<int, int>();
        private Dictionary<int, IEvent> subEvents = new Dictionary<int, IEvent>();
        private Dictionary<int, IAttrProperty> attrProperties = new Dictionary<int, IAttrProperty>();
        private TypeManager types = new TypeManager();

        public ITypeManager Types {
            get {
                return types;
            }
        }

        internal AttrModel AttributeModel { get; set; }

        internal IReadOnlyDictionary<int, IEvent> Values {
            get {
                return subEvents;
            }
        }

        internal void Register(IEventData e) {
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

        internal void AddValue<T>(int id, T value) where T : IEvent {
            subEvents.Add(id, value);
        }

        internal bool IsEventAncestor(int ancestor, int checkValue) {
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

        public bool PropertyRegistered(int id) {
            return attrProperties.ContainsKey(id);
        }

        public bool TryGetEvent(int id, out IEventData value) {
            if (events.TryGetValue(id, out value)) {
                return true;
            }

            value = null;
            return false;
        }

        public bool TryGetValue<T>(int id, out T value) where T : IEvent {
            IEvent subEvent;

            if (!subEvents.TryGetValue(id, out subEvent)) {
                value = default(T);
                return false;
            }

            if (!(subEvent is T)) {
                value = default(T);
                return false;
            }

            value = (T)subEvent;
            return true;
        }

        internal void AddPendingAttrProperty(IAttrProperty prop) {
            attrProperties.Add(prop.ID, prop);
        }

        internal AttrProperty GetPendingAttrProperty(int id) {
            return (AttrProperty)attrProperties[id];
        }

        internal void FlushAttrProperties(int propertyID) {
            IAttrProperty property;

            if (!attrProperties.TryGetValue(propertyID, out property)) {
                return;
            }

            foreach (var kv in property.Values) {
                AttributeModel.RegisterValue(propertyID, kv.Key, kv.Value);
            }

            attrProperties.Remove(propertyID);
        }

        private bool IsGenesisValue(IEventData e) =>
            e.ID == e.ValueID && events.Count == 0;
    }
}