using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal sealed class Entity : Event, IEntity {
        public int EntityID => EventValue.ID;

        public string Label => EventValue.Value;

        public Entity(IDataContext context, IEventData e) : base(context, e) {

        }

        public async Task<IEnumerable<IModel>> GetModels() =>
            await Context.Storage.GetEntityModels(EntityID);

        public async Task<IEnumerable<IIndividual>> GetIndividuals() =>
            await Context.Storage.GetEntityIndividuals(EntityID);
    }
}