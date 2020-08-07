using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal sealed class AttrConstraint : Event, IAttrConstraint {
        public int ConstraintID => EventValue.ID;
        public int AttributeID => EventValue.BaseEventID;
        public string ConstraintValue => EventValue.Value;

        public AttrConstraint(IDataContext context, IEventData e) : base(context, e) {

        }

        public async Task<bool> ContainsValueCandidate(int individualID) {
            var values = await GetValueCandidates();
            return values.Any(v => v.IndividualID == individualID);
        }

        public async Task<IEnumerable<IIndividual>> GetValueCandidates() =>
            await Context.Storage.GetAttributeConstraintValues(ConstraintID);
    }
}