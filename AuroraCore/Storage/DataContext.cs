using AuroraCore.Types;

namespace AuroraCore.Storage {
    public interface IDataContext {
        IStorageAPI Storage { get; }
        ITypeManager Types { get; }
    }

    internal class DataContext : IDataContext {
        public IStorageAPI Storage { get; private set; }
        public ITypeManager Types { get; private set; }

        public DataContext(IStorageAPI storage, ITypeManager typeManager) {
            Storage = storage;
            Types = typeManager;
        }
    }
}