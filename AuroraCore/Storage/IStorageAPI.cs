using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage {
    public interface IStorageAPI {
        Task<IEvent> GetEvent(int id);
        Task<IEnumerable<IEvent>> GetEvents(int offset = 0, int limit = 10);
        Task<IAttr> GetAttribute(int id);
        Task<IEnumerable<IAttr>> GetAttributes();
        Task<IAttrModel> GetAttrModel();
        Task<IAttrProperty> GetAttrProperty(int propertyID);
        Task<IEnumerable<IAttrProperty>> GetAttrProperties();
        Task<IAttrPropertyMember> GetAttrPropertyMember(int attrID, int propertyID);
        Task<IEnumerable<IAttrPropertyMember>> GetAttrPropertyMembers(int attrID);
        Task<IEnumerable<IIndividual>> GetAttrPropertyValues(int propertyID);
        Task<IEvent> GetAttrValue(int attrID, int valueID);
        Task<IEnumerable<IEvent>> GetAttrValues(int attrID);
        Task<IModel> GetModel(int id);
        Task<IEnumerable<IModel>> GetModels();
        Task<IModelProperty<IAttr>> GetModelAttribute(int modelID, int attrID);
        Task<IEnumerable<IModelProperty<IAttr>>> GetModelAttributes(int modelID);
        Task<IModelProperty<IRelation>> GetModelRelation(int modelID, int relationID);
        Task<IEnumerable<IModelProperty<IRelation>>> GetModelRelations(int modelID);
        Task<IEvent> GetModelPropertyValueProperty(int modelID, int attributeID, int valuePropertyID);
        Task<IEnumerable<IEvent>> GetModelPropertyValueProperties(int modelID, int attributeID);
        Task<IIndividual> GetIndividual(int id);
        Task<IEnumerable<IIndividual>> GetIndividuals();
        Task<IEnumerable<string>> GetIndividualAttribute(int individualID, int attributeID);
        Task<IReadOnlyDictionary<int, IEnumerable<string>>> GetIndividualAttributes(int id);
        Task<IEnumerable<string>> GetIndividualRelation(int individualID, int relationID);
        Task<IReadOnlyDictionary<int, IEnumerable<string>>> GetIndividualRelations(int id);
        Task<IEnumerable<IIndividual>> GetActors();
        Task<IEnumerable<IIndividual>> GetRoles();
        Task<IEntity> GetEntity(int entityID);
        Task<IEnumerable<IEntity>> GetEntities();
        Task<IEnumerable<IModel>> GetEntityModels(int id);
        Task<IEnumerable<IIndividual>> GetEntityIndividuals(int id);
        Task<IRelation> GetRelation(int id);
        Task<IEnumerable<IRelation>> GetRelations();
        Task<IEnumerable<IIndividual>> GetRelationCandidates();
        Task<bool> IsEventAncestor(int ancestor, int checkValue);
        Task<IIndividual> GetDataTypeIndividual(string name);
        DataType GetDataType(string name);
    }
}