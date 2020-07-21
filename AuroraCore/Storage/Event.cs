using System;

namespace AuroraCore.Storage {
    public interface IEvent {
        int ID { get; }
        int BaseEventID { get; }
        int ValueID { get; }
        int ConditionEventID { get; }
        int ActorEventID { get; }
        string Value { get; }
        DateTime Date { get; }
    }

    internal abstract class Event : IEvent {
        public int ID { get; private set; }
        public int BaseEventID { get; private set; }
        public int ValueID { get; private set; }
        public int ConditionEventID { get; private set; }
        public int ActorEventID { get; private set; }
        public string Value { get; private set; }
        public DateTime Date { get; private set; }

        protected IDataContext Context { get; private set; }

        public Event(IDataContext context, IEvent e) {
            ID = e.ID;
            BaseEventID = e.BaseEventID;
            ValueID = e.ValueID;
            ConditionEventID = e.ConditionEventID;
            ActorEventID = e.ActorEventID;
            Value = e.Value;
            Context = context;
        }
    }
}