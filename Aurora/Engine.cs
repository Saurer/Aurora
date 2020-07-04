using AuroraCore;

namespace Aurora {
    public static class Engine {
        private static EngineBase instance;
        public static EngineBase Instance {
            get {
                if (null == instance) {
                    instance = new EngineBase();
                }

                return instance;
            }
        }
    }
}