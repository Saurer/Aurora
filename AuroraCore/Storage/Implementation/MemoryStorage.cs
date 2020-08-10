using System.Collections.Generic;
using AuroraCore.Types;

namespace AuroraCore.Storage.Implementation {
    public partial class MemoryStorage : IStorageAdapter {
        private Dictionary<int, IEventData> events = new Dictionary<int, IEventData>();

        // Each key represents a collection of descendants(any level)
        private Dictionary<int, List<int>> subEventAncestors = new Dictionary<int, List<int>>();

        // Each key represents a direct descendant of a parent value
        private Dictionary<int, int> subEventChildren = new Dictionary<int, int>();

        // Each key represent a direct connection from container to provider
        private Dictionary<int, int> containerProviders = new Dictionary<int, int>();

        private IDataContext context;

        public MemoryStorage(ITypeManager typeManager) {
            context = new DataContext(this, typeManager);
        }
    }
}