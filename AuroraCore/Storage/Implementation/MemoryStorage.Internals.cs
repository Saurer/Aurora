using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    public partial class MemoryStorage : IStorageAdapter {
        public async Task Rollback(int positionID) {
            var positionEvent = await GetEvent(positionID);
            events = events
                .Where(kv => kv.Value.Date > positionEvent.Date)
                .ToDictionary(k => k.Key, v => v.Value);
        }

        public async Task AddEvent(IEventData e) {
            await Task.Yield();
            events.Add(e.ID, e);
        }

        public async Task Prune() {
            await Task.Yield();
            events = new Dictionary<int, IEventData>();
        }

        public async Task<bool> IsEventAncestor(int ancestor, int checkValue) {
            await Task.Yield();

            if (ancestor == StaticEvent.Event || ancestor == checkValue) {
                return true;
            }

            if (subEventAncestors.TryGetValue(ancestor, out var values)) {
                return values.Contains(checkValue);
            }

            return false;
        }

        public async Task AddSubEvent(int ancestorID, int childID) {
            if (ancestorID == StaticEvent.Event) {
                return;
            }

            if (!subEventAncestors.ContainsKey(ancestorID)) {
                subEventAncestors[ancestorID] = new List<int>();
            }

            subEventAncestors[ancestorID].Add(childID);
            subEventChildren[childID] = ancestorID;

            if (subEventChildren.TryGetValue(ancestorID, out var ancestorDirectParent)) {
                await AddSubEvent(ancestorDirectParent, ancestorID);
            }
        }

        public async Task AddContainer(int containerID, int providerID) {
            await Task.Yield();
            containerProviders[containerID] = providerID;
        }
    }
}