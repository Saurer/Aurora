using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage {
    public interface IStorageAdapter {
        Task AddEvent(IEvent value);
        Task<IEvent> GetEvent(int id);
        Task<IEnumerable<IEvent>> GetEvents(int offset = 0, int limit = 10);
        Task<IAttr> GetAttribute(int id);
        Task<IAttr> GetModelAttribute(int modelID, int attrID);
        Task<IEnumerable<IAttr>> GetModelAttributes(int modelID);
        Task<IAttrModel> GetAttrModel();
        Task<IAttrProperty> GetAttrProperty(int propertyID);
        Task<IEnumerable<IAttrProperty>> GetAttrProperties();
        Task<IAttrPropertyMember> GetAttrPropertyMember(int attrID, int propertyID);
        Task<IEnumerable<IAttrPropertyMember>> GetAttrPropertyMembers(int attrID);
        Task<IEnumerable<IIndividual>> GetAttrPropertyValues(int propertyID);
        Task<IModel> GetModel(int id);
        Task<IEnumerable<IModel>> GetModels();
        Task<IIndividual> GetIndividual(int id);
        Task<IEnumerable<IIndividual>> GetIndividuals();
        Task<IReadOnlyDictionary<int, string>> GetIndividualAttributes(int id);
        Task<bool> IsEventAncestor(int ancestor, int checkValue);
        DataType GetDataType(string name);
    }
}