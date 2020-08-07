namespace AuroraCore.Storage {
    public interface IBoxedValue {
        string PlainValue { get; }
        string ShownValue { get; }
    }
}