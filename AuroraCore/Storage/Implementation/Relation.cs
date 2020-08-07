using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal sealed class Relation : Event, IRelation {
        public int PropertyID => EventValue.ID;
        public string Label => EventValue.Value;
        public IIndividual PropertyIndividual { get; private set; }

        public Relation(IDataContext context, IEventData e) : base(context, e) {
            PropertyIndividual = new Individual(context, e);
        }

        public async Task<IEnumerable<IIndividual>> GetValueCandidates() =>
            await Context.Storage.GetRelationValueCandidates();
    }
}