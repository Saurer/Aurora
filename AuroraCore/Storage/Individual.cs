using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IIndividual : IEvent {
        Task<IModel> GetModel();
        Task<IIndividual> GetActor();
        Task<IReadOnlyDictionary<int, string>> GetAttributes();
    }

    internal sealed class Individual : Event, IIndividual {
        public Individual(IDataContext context, IEvent e) : base(context, e) {

        }

        public async Task<IReadOnlyDictionary<int, string>> GetAttributes() {
            return await Context.Storage.GetIndividualAttributes(ID);
        }

        public async Task<IIndividual> GetActor() {
            return await Context.Storage.GetIndividual(ActorEventID);
        }

        public async Task<IModel> GetModel() {
            return await Context.Storage.GetModel(ConditionEventID);
        }
    }
}