using AuroraCore;
using AuroraCore.Storage.Implementation;
using AuroraCore.Types;

namespace Aurora {
    public static class Engine {
        private static EngineBase instance;
        public static EngineBase Instance {
            get {
                if (null == instance) {
                    var typeManager = new TypeManager();
                    var storageAdapter = new MemoryStorage(typeManager);
                    instance = new EngineBase(storageAdapter);
                }

                return instance;
            }
        }
    }
}