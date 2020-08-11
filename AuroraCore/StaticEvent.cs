namespace AuroraCore {
    public static class StaticEvent {
        public const int Event = 0;
        public const int EventModel = 9;
        public const int SubEvent = 1;
        public const int Actor = 2;
        public const int ActorModel = 20;
        public const int Entity = 3;
        public const int EntityModel = 10;
        public const int Relation = 4;
        public const int RelationModel = 11;
        public const int Attribute = 5;
        public const int AttributeConstraint = 6;
        public const int AttributeModel = 15;
        public const int AttributeValue = 26; // FIXME: Must me 31 after relations implementation
        public const int Model = 7;
        public const int Individual = 8;
        public const int DataType = 12;
        public const int DataTypeModel = 13;
        public const int Role = 23;
        public const int RoleModel = 24;

        // FIXME: Must me 36,37,38 after relations implementation
        public const int ValueProperty = 27;
        public const int Cardinality = 28;
        public const int Required = 29;
        public const int Permission = 30;
    }
}