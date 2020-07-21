using AuroraCore.Types;

namespace AuroraCore.Storage {
    public interface IDataContext {
        IStorageAdapter Storage { get; }
        ITypeManager Types { get; }
    }

    internal class DataContext : IDataContext {
        public IStorageAdapter Storage { get; private set; }
        public ITypeManager Types { get; private set; }

        public DataContext(IStorageAdapter storage, ITypeManager typeManager) {
            Storage = storage;
            Types = typeManager;
        }
    }
}