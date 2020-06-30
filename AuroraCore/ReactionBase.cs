using System.Reflection;

namespace AuroraCore {
    public struct ReactionBase {
        public int EventID { get; private set; }
        public MethodInfo Method { get; private set; }

        public ReactionBase(int eventID, MethodInfo method) {
            EventID = eventID;
            Method = method;
        }
    }
}