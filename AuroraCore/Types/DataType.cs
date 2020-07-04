namespace AuroraCore.Types {
    public abstract class DataType {
        public string Name { get; set; }
        public abstract bool Validate(string data);
    }
}