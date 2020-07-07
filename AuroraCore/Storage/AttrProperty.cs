using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IAttrProperty : IEvent {
        Task<IReadOnlyDictionary<int, string>> GetValues();
        Task<bool> ContainsValue(int id);
    }

    internal sealed class AttrProperty : Event, IAttrProperty {
        public AttrProperty(IDataContext context, IEvent e) : base(context, e) {

        }

        public Task<bool> ContainsValue(int id) {
#warning TODO: Implementation
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyDictionary<int, string>> GetValues() {
#warning TODO: Implementation
            throw new System.NotImplementedException();
        }
    }
}