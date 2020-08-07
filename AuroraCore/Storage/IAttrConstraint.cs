using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IAttrConstraint : IEvent {
        int ConstraintID { get; }
        int AttributeID { get; }
        string ConstraintValue { get; }
        Task<IEnumerable<IIndividual>> GetValueCandidates();
        Task<bool> ContainsValueCandidate(int individualID);
    }
}