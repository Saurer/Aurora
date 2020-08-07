using System;

namespace AuroraCore.Storage {
    public interface IEventData {
        int ID { get; }
        int BaseEventID { get; }
        int ValueID { get; }
        int ConditionEventID { get; }
        int ActorEventID { get; }
        string Value { get; }
        DateTime Date { get; }
    }
}