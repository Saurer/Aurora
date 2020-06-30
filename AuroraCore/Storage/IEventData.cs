using System;

namespace AuroraCore.Storage {
    public interface IEventData {
        int ID { get; set; }
        int BaseEventID { get; set; }
        int ValueID { get; set; }
        int ConditionEventID { get; set; }
        int ActorEventID { get; set; }
        string Value { get; set; }
        DateTime Date { get; set; }
    }
}