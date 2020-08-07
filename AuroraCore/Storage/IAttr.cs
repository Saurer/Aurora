using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage {
    public interface IAttr : IProperty {
        string Label { get; }
        Task<bool> IsBoxed();
        Task<DataType> GetDataType();
        Task<IAttachedAttrConstraint> GetConstraint(int id);
        Task<IEnumerable<IAttachedAttrConstraint>> GetConstraints();
        Task<IEnumerable<IEvent>> GetValueCandidates();
        Task<bool> Validate(string value);
    }
}