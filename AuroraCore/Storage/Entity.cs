using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IEntity : IEvent {
        Task<IEnumerable<IModel>> GetModels();
        Task<IEnumerable<IIndividual>> GetIndividuals();
    }

    internal sealed class Entity : Event, IEntity {
        public Entity(IDataContext context, IEvent e) : base(context, e) {

        }

        public async Task<IEnumerable<IModel>> GetModels() {
            return await Context.Storage.GetEntityModels(ID);
        }

        public async Task<IEnumerable<IIndividual>> GetIndividuals() {
            return await Context.Storage.GetEntityIndividuals(ID);
        }
    }
}