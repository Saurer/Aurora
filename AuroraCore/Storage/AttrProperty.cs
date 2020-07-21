using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IAttrProperty : IEvent {
        Task<IEnumerable<IIndividual>> GetValues();
        Task<bool> ContainsValue(int id);
    }

    internal sealed class AttrProperty : Event, IAttrProperty {
        public AttrProperty(IDataContext context, IEvent e) : base(context, e) {

        }

        public async Task<bool> ContainsValue(int id) {
            var values = await GetValues();
            return values.Any(v => v.ID == id);
        }

        public Task<IEnumerable<IIndividual>> GetValues() {
            return Context.Storage.GetAttrPropertyValues(ID);
        }
    }
}