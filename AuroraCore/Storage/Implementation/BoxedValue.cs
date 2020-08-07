namespace AuroraCore.Storage.Implementation {
    internal class BoxedValue : IBoxedValue {
        public string PlainValue { get; private set; }
        public string ShownValue { get; private set; }

        public BoxedValue(string plainValue, string shownValue) {
            PlainValue = plainValue;
            ShownValue = shownValue;
        }
    }
}