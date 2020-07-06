using System;
using AuroraCore.Storage;

namespace AuroraCore.Events {
    public interface IEvent : IEventData {
    }

    internal abstract class Event : IEvent {
        public int ID { get; private set; }
        public int BaseEventID { get; private set; }
        public int ValueID { get; private set; }
        public int ConditionEventID { get; private set; }
        public int ActorEventID { get; private set; }
        public string Value { get; private set; }
        public DateTime Date { get; private set; }

        public Event(IEventData e) {
            ID = e.ID;
            BaseEventID = e.BaseEventID;
            ValueID = e.ValueID;
            ConditionEventID = e.ConditionEventID;
            ActorEventID = e.ActorEventID;
            Value = e.Value;
        }
    }
}