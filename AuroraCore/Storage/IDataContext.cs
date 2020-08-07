using AuroraCore.Types;

namespace AuroraCore.Storage {
    internal interface IDataContext {
        IStorageAPI Storage { get; }
        ITypeManager Types { get; }
    }
}