using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IIndividual : IEvent {
        Task<IModel> GetModel();
        Task<IIndividual> GetActor();
        Task <IEnumerable<string>> GetAttribute(int attrID);
        Task<IReadOnlyDictionary<int, IEnumerable<string>>> GetAttributes();
    }

    internal sealed class Individual : Event, IIndividual {
        public Individual(IDataContext context, IEvent e) : base(context, e) {

        }

        public async Task<IEnumerable<string>> GetAttribute(int attributeID) {
            return await Context.Storage.GetIndividualAttribute(ID, attributeID);
        }

        public async Task<IReadOnlyDictionary<int, IEnumerable<string>>> GetAttributes() {
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