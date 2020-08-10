namespace AuroraCore.Storage {
    public interface IBoxedValue : IEvent {
        int AssignationID { get; }
        string PlainValue { get; }
        string ShownValue { get; }
    }
}