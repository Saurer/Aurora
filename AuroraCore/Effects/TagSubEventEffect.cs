using System.Threading.Tasks;
using AuroraCore.Storage;

namespace AuroraCore.Effects {
    public class TagSubEventEffect : Effect {
        private readonly int baseEventID;
        private readonly int childEventID;

        public TagSubEventEffect(int baseEventID, int childEventID) {
            this.baseEventID = baseEventID;
            this.childEventID = childEventID;
        }

        public override async Task Execute(IStorageAdapter storage) =>
            await storage.AddSubEvent(baseEventID, childEventID);
    }
}