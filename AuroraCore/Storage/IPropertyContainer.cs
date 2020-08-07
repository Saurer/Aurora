using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IPropertyContainer {
        Task<IEnumerable<IBoxedValue>> GetAttribute(int attrID);
        Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetAttributes();
        Task<IEnumerable<IBoxedValue>> GetRelation(int relationID);
        Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetRelations();
        Task<bool> Validate();
    }
}