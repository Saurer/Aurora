using AuroraCore.Storage;

namespace AuroraCore.Types {
    public abstract class DataType {
        public abstract string Name { get; }
        public abstract bool Validate(string data);
        public virtual bool AllowsBoxedValue(IEvent e) {
            return false;
        }
    }
}