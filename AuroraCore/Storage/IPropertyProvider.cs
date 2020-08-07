using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IPropertyProvider {
        Task<IAttachedProperty<IAttr>> GetAttribute(int attributeID);
        Task<IAttachedProperty<IRelation>> GetRelation(int relationID);
        Task<IEnumerable<IAttachedProperty<IAttr>>> GetAttributes();
        Task<IEnumerable<IAttachedProperty<IRelation>>> GetRelations();
    }
}