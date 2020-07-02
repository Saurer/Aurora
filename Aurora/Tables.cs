using System.Collections.Generic;

namespace Aurora {
    public static class Tables {
        public static IEnumerable<EventData> Table = new EventData[]{
            new EventData{ ID = 0, BaseEventID = 0, ValueID = 0, Value = "Event", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 1, BaseEventID = 0, ValueID = 0, Value = "SubEvent", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 2, BaseEventID = 0, ValueID = 1, Value = "Actor", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 3, BaseEventID = 0, ValueID = 1, Value = "Entity", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 4, BaseEventID = 0, ValueID = 1, Value = "Relation", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 5, BaseEventID = 0, ValueID = 1, Value = "Attribute", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 6, BaseEventID = 0, ValueID = 1, Value = "AttributeProperty", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 7, BaseEventID = 0, ValueID = 1, Value = "Model", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 8, BaseEventID = 0, ValueID = 1, Value = "Individual", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 9, BaseEventID = 0, ValueID = 7, Value = "Model_Event", ConditionEventID = 0, ActorEventID = 0 },
            new EventData{ ID = 10, BaseEventID = 3, ValueID = 7, Value = "Model_Entity", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 11, BaseEventID = 4, ValueID = 7, Value = "Model_Relation", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 12, BaseEventID = 6, ValueID = 1, Value = "DataType", ConditionEventID = 6, ActorEventID = 0 },
            new EventData{ ID = 13, BaseEventID = 12, ValueID = 7, Value = "Model_DataType", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 14, BaseEventID = 12, ValueID = 8, Value = "basic_type", ConditionEventID = 13, ActorEventID = 0 },
            new EventData{ ID = 15, BaseEventID = 5, ValueID = 7, Value = "Model_Attribute", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 16, BaseEventID = 15, ValueID = 12, Value = "12", ConditionEventID = 15, ActorEventID = 0 },
            new EventData{ ID = 17, BaseEventID = 5, ValueID = 8, Value = "Name", ConditionEventID = 15, ActorEventID = 0 },
            new EventData{ ID = 18, BaseEventID = 17, ValueID = 12, Value = "14", ConditionEventID = 17, ActorEventID = 0 },
            new EventData{ ID = 19, BaseEventID = 9, ValueID = 5, Value = "17", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 20, BaseEventID = 2, ValueID = 7, Value = "Actor_Model", ConditionEventID = 9, ActorEventID = 0 },
            new EventData{ ID = 21, BaseEventID = 2, ValueID = 8, Value = "Actor_Main", ConditionEventID = 20, ActorEventID = 0 },
            new EventData{ ID = 22, BaseEventID = 21, ValueID = 17, Value = "Main Actor", ConditionEventID = 21, ActorEventID = 0 },
        };

        public static IEnumerable<EventData> OldTable = new EventData[]{
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
    }
}