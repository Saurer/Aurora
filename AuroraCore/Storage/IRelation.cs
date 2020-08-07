using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IRelation : IProperty {
        string Label { get; }
        IIndividual PropertyIndividual { get; }
        Task<IEnumerable<IIndividual>> GetValueCandidates();
    }
}