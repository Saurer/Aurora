using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage {
    public interface IStorageAPI {
        #region Event
        Task<IEvent> GetEvent(int id);
        Task<IEnumerable<IEvent>> GetEvents(int offset = 0, int limit = 10);
        #endregion

        #region Attribute
        Task<IAttr> GetAttribute(int attributeID);
        Task<IEnumerable<IAttr>> GetAttributes();
        Task<IAttrConstraint> GetAttributeConstraint(int constraintID);
        Task<IEnumerable<IAttrConstraint>> GetAttributeConstraints();
        Task<IEnumerable<IIndividual>> GetAttributeConstraintValues(int constraintID);
        Task<IAttachedAttrConstraint> GetAttributeAttachedConstraint(int attributeID, int constraintID);
        Task<IEnumerable<IAttachedAttrConstraint>> GetAttributeAttachedConstraints(int attributeID);
        Task<IEvent> GetAttributeValueCandidate(int attributeID, int individualID);
        Task<IEnumerable<IEvent>> GetAttributeValueCandidates(int attributeID);
        #endregion

        #region Relation
        Task<IRelation> GetRelation(int relationID);
        Task<IEnumerable<IRelation>> GetRelations();
        Task<IEnumerable<IIndividual>> GetRelationValueCandidates();
        #endregion

        #region Model
        Task<IModel> GetModel(int modelID);
        Task<IEnumerable<IModel>> GetModels();
        #endregion

        #region Property provider
        Task<IPropertyProvider> GetPropertyProvider(int providerID);
        Task<IAttachedProperty<IAttr>> GetPropertyProviderAttribute(int providerID, int attrID);
        Task<IEnumerable<IAttachedProperty<IAttr>>> GetPropertyProviderAttributes(int providerID);
        Task<IAttachedProperty<IRelation>> GetPropertyProviderRelation(int providerID, int relationID);
        Task<IEnumerable<IAttachedProperty<IRelation>>> GetPropertyProviderRelations(int providerID);
        Task<IEvent> GetPropertyProviderValueConstraint(int providerID, int propertyID, int constraintID);
        Task<IEnumerable<IEvent>> GetPropertyProviderValueConstraints(int providerID, int propertyID);
        #endregion

        #region Property container
        Task<IPropertyContainer> GetPropertyContainer(int containerID);
        Task<IPropertyProvider> GetContainerPropertyProvider(int containerID);
        Task<IEnumerable<IBoxedValue>> GetPropertyContainerAttribute(int containerID, int attributeID);
        Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetPropertyContainerAttributes(int containerID);
        Task<IEnumerable<IBoxedValue>> GetPropertyContainerRelation(int containerID, int relationID);
        Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetPropertyContainerRelations(int containerID);
        #endregion

        #region Entity
        Task<IEntity> GetEntity(int entityID);
        Task<IEnumerable<IEntity>> GetEntities();
        Task<IEnumerable<IModel>> GetEntityModels(int entityID);
        Task<IEnumerable<IIndividual>> GetEntityIndividuals(int entityID);
        #endregion

        #region Individual
        Task<IIndividual> GetIndividual(int individualID);
        Task<IEnumerable<IIndividual>> GetIndividuals();
        #endregion


        #region Actor
        Task<IIndividual> GetActor(int actorID);
        Task<IEnumerable<IIndividual>> GetActors();
        #endregion

        #region Role
        Task<IIndividual> GetRole(int roleID);
        Task<IEnumerable<IIndividual>> GetRoles();
        #endregion

        #region Other
        Task<IIndividual> GetDataTypeIndividual(string name);
        DataType GetDataType(string name);
        #endregion
    }
}