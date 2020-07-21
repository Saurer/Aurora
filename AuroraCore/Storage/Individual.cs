using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IIndividual : IEvent {
        Task<IModel> GetModel();
        Task<IReadOnlyDictionary<int, string>> GetAttributes();
    }

    internal sealed class Individual : Event, IIndividual {
        public Individual(IDataContext context, IEvent e) : base(context, e) {

        }

        public Task<IReadOnlyDictionary<int, string>> GetAttributes() {
            return Context.Storage.GetIndividualAttributes(ID);
        }

        public Task<IModel> GetModel() {
            return Context.Storage.GetModel(ConditionEventID);
        }
    }
}