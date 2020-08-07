using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IEntity : IEvent {
        int EntityID { get; }
        string Label { get; }

        Task<IEnumerable<IModel>> GetModels();
        Task<IEnumerable<IIndividual>> GetIndividuals();
    }
}