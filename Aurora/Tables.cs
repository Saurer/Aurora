using System.Collections.Generic;
using AuroraCore;

namespace Aurora {
    public static class Tables {
        public static IEnumerable<EventData> Table = new EventData[]{
            new EventData(StaticEvent.Event, StaticEvent.Event, StaticEvent.Event, StaticEvent.Event, StaticEvent.Event, "Event"),
            new EventData(StaticEvent.SubEvent, StaticEvent.Event, StaticEvent.Event, StaticEvent.Event, StaticEvent.Event, "SubEvent"),

            SubEvent(StaticEvent.Actor, StaticEvent.Event, "Actor", StaticEvent.Event),
            SubEvent(StaticEvent.Entity, StaticEvent.Event, "Entity", StaticEvent.Event),
            SubEvent(StaticEvent.Relation, StaticEvent.Event, "Relation", StaticEvent.Event),
            SubEvent(StaticEvent.Attribute, StaticEvent.Event, "Attribute", StaticEvent.Event),
            SubEvent(StaticEvent.AttributeProperty, StaticEvent.Event, "AttributeProperty", StaticEvent.Event),
            SubEvent(StaticEvent.Model, StaticEvent.Event, "Model", StaticEvent.Event),
            SubEvent(StaticEvent.Individual, StaticEvent.Event, "Individual", StaticEvent.Event),
            SubEvent(StaticEvent.Role, StaticEvent.Event, "Role", StaticEvent.Event),
            SubEvent(StaticEvent.ValueProperty, StaticEvent.Event, "ValueProperty", StaticEvent.Event),

            SubEvent(StaticEvent.DataType, StaticEvent.AttributeProperty, "DataType", StaticEvent.AttributeProperty),
            SubEvent(StaticEvent.AttributeValue, StaticEvent.Event, "AttributeValue", StaticEvent.AttributeProperty),

            SubEvent(StaticEvent.Cardinality, StaticEvent.ValueProperty, "Cardinality", StaticEvent.ValueProperty),
            SubEvent(StaticEvent.Required, StaticEvent.ValueProperty, "Required", StaticEvent.ValueProperty),

            Model(StaticEvent.EventModel, StaticEvent.Event, "Model_Event", StaticEvent.Event),
            Model(StaticEvent.EntityModel, StaticEvent.Entity, "Model_Entity", StaticEvent.EventModel),
            Model(StaticEvent.RelationModel, StaticEvent.Relation, "Model_Relation", StaticEvent.EventModel),
            Model(StaticEvent.DataTypeModel, StaticEvent.DataType, "Model_DataType", StaticEvent.EventModel),
            Model(StaticEvent.AttributeModel, StaticEvent.Attribute, "Model_Attribute", StaticEvent.EventModel),
            Model(StaticEvent.ActorModel, StaticEvent.Actor, "Model_Actor", StaticEvent.EventModel),
            Model(StaticEvent.RoleModel, StaticEvent.Role, "Model_Role", StaticEvent.EventModel),

            Individual(14, StaticEvent.DataType, "basic_type", StaticEvent.DataTypeModel),
            AttributeProperty(16, 14),

            Individual(25, StaticEvent.DataType, "enum_type", StaticEvent.DataTypeModel),

            Individual(17, StaticEvent.Attribute, "Name", StaticEvent.AttributeModel),
            DataType(18, 17, 14),
            ModelAttribute(19, StaticEvent.EventModel, 17),

            Individual(21, StaticEvent.Actor, "Actor_Main", StaticEvent.ActorModel),
            IndividualAttribute(22, 21, 17, "Main Actor"),
        };

        private static EventData SubEvent(int id, int baseEventID, string value, int conditionEventID) =>
            new EventData(id, baseEventID, StaticEvent.SubEvent, conditionEventID, StaticEvent.Event, value);

        private static EventData Model(int id, int baseEventID, string value, int parentModelID) =>
            new EventData(id, baseEventID, StaticEvent.Model, parentModelID, StaticEvent.Event, value);

        private static EventData Individual(int id, int baseEventID, string name, int modelID) =>
            new EventData(id, baseEventID, StaticEvent.Individual, modelID, StaticEvent.Event, name);

        private static EventData AttributeProperty(int id, int propertyID) =>
            new EventData(id, StaticEvent.AttributeModel, StaticEvent.AttributeProperty, StaticEvent.AttributeModel, StaticEvent.Event, propertyID.ToString());

        private static EventData DataType(int id, int attributeID, int dataTypeID) =>
            new EventData(id, attributeID, StaticEvent.DataType, attributeID, StaticEvent.Event, dataTypeID.ToString());

        private static EventData ModelAttribute(int id, int modelID, int attributeID) =>
            new EventData(id, modelID, StaticEvent.Attribute, modelID, StaticEvent.Event, attributeID.ToString());

        private static EventData IndividualAttribute(int id, int individualID, int attributeID, string value) =>
            new EventData(id, individualID, attributeID, individualID, StaticEvent.Event, value);
    }
}