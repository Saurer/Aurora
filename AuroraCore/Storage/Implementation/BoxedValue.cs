namespace AuroraCore.Storage.Implementation {
    internal class BoxedValue : Event, IBoxedValue {
        public int AssignationID => EventValue.ID;
        public string PlainValue => EventValue.Value;
        public string ShownValue { get; private set; }

        public BoxedValue(IDataContext context, IEventData e, string shownValue) : base(context, e) {
            ShownValue = shownValue;
        }
    }
}