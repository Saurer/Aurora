using AuroraCore.Types;

namespace AuroraCore.Storage.Implementation {
    internal class DataContext : IDataContext {
        public IStorageAPI Storage { get; private set; }
        public ITypeManager Types { get; private set; }

        public DataContext(IStorageAPI storage, ITypeManager typeManager) {
            Storage = storage;
            Types = typeManager;
        }
    }
}