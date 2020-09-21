using System;

namespace AuroraCore.Storage {
    public interface IEventData {
        int ID { get; }
        int BaseEventID { get; }
        int ValueID { get; }
        ConditionRule[] Conditions { get; }
        int ActorEventID { get; }
        string Value { get; }
        DateTime Date { get; }
    }
}