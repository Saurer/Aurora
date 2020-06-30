using System;
using System.Collections.Generic;
using Aurora.Controllers;
using AuroraCore;
using AuroraCore.Storage;

namespace Aurora {
    class EventData : IEventData {
        public int ID { get; set; }
        public int BaseEventID { get; set; }
        public int ValueID { get; set; }
        public int ConditionEventID { get; set; }
        public int ActorEventID { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
    }

    class Program {
        private static IEnumerable<EventData> graph = new EventData[]{
            new EventData{ ID = 0, BaseEventID = 0, ValueID = 0, Value = "Event", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 1, BaseEventID = 0, ValueID = 0, Value = "SubEvent", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 2, BaseEventID = 0, ValueID = 1, Value = "Actor", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 3, BaseEventID = 0, ValueID = 1, Value = "Entity", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 4, BaseEventID = 0, ValueID = 1, Value = "Relation", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 5, BaseEventID = 0, ValueID = 1, Value = "Attribute", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 6, BaseEventID = 0, ValueID = 1, Value = "Model", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 7, BaseEventID = 0, ValueID = 1, Value = "Individual", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 8, BaseEventID = 0, ValueID = 1, Value = "Role", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 9, BaseEventID = 0, ValueID = 6, Value = "Model_Event", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 14, BaseEventID = 3, ValueID = 6, Value = "Model_Entity", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 15, BaseEventID = 4, ValueID = 6, Value = "Model_Relation", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 16, BaseEventID = 5, ValueID = 1, Value = "DataType", ConditionEventID = 5, ActorEventID = 0 },
            new EventData{ ID = 17, BaseEventID = 16, ValueID = 6, Value = "Model_DataType", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 18, BaseEventID = 16, ValueID = 7, Value = "basic_type", ConditionEventID = 19, ActorEventID = 0 },
            new EventData{ ID = 19, BaseEventID = 5, ValueID = 6, Value = "Model_Attribute", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 20, BaseEventID = 19, ValueID = 16, Value = "18", ConditionEventID = 19, ActorEventID = 0 },

            // new EventData{ ID = 21, BaseEventID = 5, ValueID = 1, Value = "Name", ConditionEventID = 5, ActorEventID = 0 },
            // new EventData{ ID = 22, BaseEventID = 21, ValueID = 6, Value = "Model_Name", ConditionEventID = 19, ActorEventID = 0 },
            new EventData{ ID = 21, BaseEventID = 5, ValueID = 7, Value = "Name", ConditionEventID = 5, ActorEventID = 0 },
            new EventData{ ID = 22, BaseEventID = 21, ValueID = 16, Value = "18", ConditionEventID = 21, ActorEventID = 0 },

            new EventData{ ID = 23, BaseEventID = 9, ValueID = 21, Value = "", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 24, BaseEventID = 2, ValueID = 6, Value = "Model_Actor", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 25, BaseEventID = 2, ValueID = 7, Value = "Main_Actor", ConditionEventID = 24, ActorEventID = 0 },
            new EventData{ ID = 26, BaseEventID = 25, ValueID = 21, Value = "Main actor", ConditionEventID = 25, ActorEventID = 0 },
        };

        static void Main(string[] args) {
            var engine = new EngineBase();
            engine.AddController<EventController>();

            foreach (var e in graph) {
                engine.ProcessEvent(e);
            }

            Console.WriteLine("Done");
        }
    }
}
