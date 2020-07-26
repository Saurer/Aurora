namespace AuroraCore.Types {
    public abstract class DataType {
        public virtual bool IsBoxed => false;
        public abstract string Name { get; }
        public abstract bool Validate(string data);
        public virtual bool AllowsBoxedValue(string value) {
            return false;
        }
    }
}