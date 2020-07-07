using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IStorageAdapter {
        Task AddEvent(IEvent value);
        Task<IEvent> GetEvent(int id);
        Task<IAttr> GetAttribute(int id);
        Task<IAttr> GetModelAttribute(int modelID, int attrID);
        Task<IEnumerable<IAttr>> GetModelAttributes(int modelID);
        Task<IAttrModel> GetAttrModel();
        Task<IAttrProperty> GetAttrProperty(int propertyID);
        Task<IEnumerable<IAttrProperty>> GetAttrProperties();
        Task<IAttrPropertyValue> GetAttrPropertyValue(int attrID, int propertyID);
        Task<IEnumerable<IAttrPropertyValue>> GetAttrPropertyValues(int attrID);
        Task<IModel> GetModel(int id);
        Task<IIndividual> GetIndividual(int id);
        Task<IReadOnlyDictionary<int, string>> GetIndividualAttributes(int id);
        Task<bool> IsEventAncestor(int ancestor, int checkValue);
    }
}