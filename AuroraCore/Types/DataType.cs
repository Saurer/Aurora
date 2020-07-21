namespace AuroraCore.Types {
    public abstract class DataType {
        public abstract string Name { get; }
        public abstract bool Validate(string data);
    }
}