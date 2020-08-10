using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage.Implementation {
    public partial class MemoryStorage : IStorageAdapter {
        #region Event
        public async Task<IEvent> GetEvent(int id) {
            await Task.Yield();
            if (events.TryGetValue(id, out var value)) {
                return new Event(context, value);
            }
            else {
                return null;
            }
        }

        public async Task<IEnumerable<IEvent>> GetEvents(int offset = 0, int limit = 10) {
            await Task.Yield();
            return events.Skip(offset).Take(limit).Select(e => new Event(context, e.Value));
        }
        #endregion

        #region Attribute
        public async Task<IAttr> GetAttribute(int id) {
            await Task.Yield();

            var attrDef = await GetEvent(id);

            if (attrDef.EventValue.BaseEventID != StaticEvent.Attribute) {
                return null;
            }

            return new Attr(context, attrDef.EventValue);
        }

        public async Task<IEnumerable<IAttr>> GetAttributes() {
            await Task.Yield();

            var attrEvents =
                from e in events
                where e.Value.BaseEventID == StaticEvent.Attribute && e.Value.ValueID == StaticEvent.Individual
                select e.Value;

            return attrEvents.Select(a => new Attr(context, a));
        }

        public async Task<IAttrConstraint> GetAttributeConstraint(int constraintID) {
            await Task.Yield();

            var constraintDef = (
                from e in events
                where
                    e.Value.BaseEventID == StaticEvent.AttributeConstraint &&
                    e.Value.ValueID == StaticEvent.SubEvent &&
                    e.Key == constraintID
                select e.Value
            ).SingleOrDefault();

            if (null == constraintDef) {
                return null;
            }

            return new AttrConstraint(context, constraintDef);
        }

        public async Task<IEnumerable<IAttrConstraint>> GetAttributeConstraints() {
            await Task.Yield();

            var constraintIDs =
                from e in events
                where e.Value.ValueID == StaticEvent.AttributeConstraint
                select Int32.Parse(e.Value.Value);

            var result = new List<IAttrConstraint>();
            foreach (var constraintID in constraintIDs) {
                var constraintDef = await GetEvent(constraintID);
                var constraint = new AttrConstraint(context, constraintDef.EventValue);
                result.Add(constraint);
            }

            return result;
        }

        public async Task<IEnumerable<IIndividual>> GetAttributeConstraintValues(int constraintID) {
            var individualIDs =
                from individual in events
                join baseEvent in events on individual.Value.BaseEventID equals baseEvent.Key
                where
                    individual.Value.ValueID == StaticEvent.Individual &&
                    baseEvent.Value.BaseEventID == StaticEvent.AttributeConstraint &&
                    baseEvent.Value.ValueID == StaticEvent.SubEvent
                select individual.Key;

            return await Task.WhenAll(individualIDs.Select(id => GetIndividual(id)));
        }

        public async Task<IAttachedAttrConstraint> GetAttributeAttachedConstraint(int attributeID, int constraintID) {
            var attrProto = await GetAttribute(attributeID);

            if (null == attrProto) {
                return null;
            }

            var constraintDef = (
                from attach in events
                join type in events on attach.Value.ValueID equals type.Key
                where
                    attach.Value.BaseEventID == attributeID &&
                    type.Value.BaseEventID == StaticEvent.AttributeConstraint &&
                    type.Key == constraintID
                select attach.Value
            ).SingleOrDefault();

            if (null == constraintDef) {
                return null;
            }

            return new AttachedAttrConstraint(context, constraintDef);
        }

        public async Task<IEnumerable<IAttachedAttrConstraint>> GetAttributeAttachedConstraints(int attributeID) {
            var attrProto = await GetAttribute(attributeID);

            if (null == attrProto) {
                return Array.Empty<IAttachedAttrConstraint>();
            }

            var properties =
                from constraint in events
                join type in events on constraint.Value.ValueID equals type.Key
                where constraint.Value.BaseEventID == attributeID && type.Value.BaseEventID == StaticEvent.AttributeConstraint
                select constraint.Value;

            return properties.Select(p => new AttachedAttrConstraint(context, p));
        }

        public async Task<IEvent> GetAttributeValueCandidate(int attributeID, int individualID) {
            await Task.Yield();

            var attrValue = (
                from e in events
                where e.Value.BaseEventID == attributeID &&
                    e.Value.ValueID == StaticEvent.AttributeValue &&
                    e.Key == individualID
                select e.Value
            ).SingleOrDefault();

            return new Event(context, attrValue);
        }

        public async Task<IEnumerable<IEvent>> GetAttributeValueCandidates(int attributeID) {
            await Task.Yield();

            var attrValues = (
                from e in events
                where e.Value.BaseEventID == attributeID &&
                    e.Value.ValueID == StaticEvent.AttributeValue
                select new Event(context, e.Value)
            );

            return attrValues;
        }
        #endregion

        #region Relation
        public async Task<IRelation> GetRelation(int relationID) {
            var relationDef = await GetEvent(relationID);

            if (relationDef.EventValue.ValueID != StaticEvent.Individual || relationDef.EventValue.BaseEventID != StaticEvent.Relation) {
                return null;
            }

            return new Relation(context, relationDef.EventValue);
        }

        public async Task<IEnumerable<IRelation>> GetRelations() {
            var relationIDs =
                from e in events
                where
                    e.Value.ValueID == StaticEvent.Individual &&
                    e.Value.BaseEventID == StaticEvent.Relation
                select e.Key;

            return await Task.WhenAll(relationIDs.Select(id => GetRelation(id)));
        }

        public async Task<IEnumerable<IIndividual>> GetRelationValueCandidates() {
            var individualIDs =
                from e in events
                join parent in events on e.Value.BaseEventID equals parent.Key
                where
                    e.Value.ValueID == StaticEvent.Individual &&
                    (parent.Value.BaseEventID == StaticEvent.Entity || e.Value.BaseEventID == StaticEvent.Actor)
                select e.Key;

            return await Task.WhenAll(individualIDs.Select(id => GetIndividual(id)));
        }
        #endregion

        #region Model
        public async Task<IModel> GetModel(int id) {
            var modelDef = await GetEvent(id);

            if (modelDef.EventValue.ValueID != StaticEvent.Model) {
                return null;
            }

            return new Model(context, modelDef.EventValue);
        }

        public async Task<IEnumerable<IModel>> GetModels() {
            var modelIDs =
                from e in events
                where e.Value.ValueID == StaticEvent.Model
                select e.Key;

            return await Task.WhenAll(modelIDs.Select(id => GetModel(id)));
        }
        #endregion

        #region Property provider
        public async Task<IAttachedProperty<IAttr>> GetPropertyProviderAttribute(int providerID, int attrID) {
            await Task.Yield();

            var attr = (
                from e in events
                where
                    e.Value.BaseEventID == providerID &&
                    e.Value.ValueID == StaticEvent.Attribute &&
                    e.Value.Value == attrID.ToString()
                select e.Value
            ).SingleOrDefault();

            if (null == attr) {
                return null;
            }

            return new AttachedProperty<IAttr>(context, attr);
        }

        public async Task<IEnumerable<IAttachedProperty<IAttr>>> GetPropertyProviderAttributes(int providerID) {
            var attrIDs =
                from e in events
                where e.Value.ValueID == StaticEvent.Attribute && e.Value.BaseEventID == providerID
                select Int32.Parse(e.Value.Value);

            var attributes = await Task.WhenAll(
                attrIDs.Select(id => GetPropertyProviderAttribute(providerID, id))
            );

            return attributes;
        }

        public async Task<IAttachedProperty<IRelation>> GetPropertyProviderRelation(int providerID, int relationID) {
            await Task.Yield();

            var relation = (
                from e in events
                where
                    e.Value.BaseEventID == providerID &&
                    e.Value.ValueID == StaticEvent.Relation &&
                    e.Value.Value == relationID.ToString()
                select e.Value
            ).SingleOrDefault();

            if (null == relation) {
                return null;
            }

            return new AttachedProperty<IRelation>(context, relation);
        }

        public async Task<IEnumerable<IAttachedProperty<IRelation>>> GetPropertyProviderRelations(int providerID) {
            await Task.Yield();

            var relationIDs =
                from e in events
                where e.Value.BaseEventID == providerID && e.Value.ValueID == StaticEvent.Relation
                select Int32.Parse(e.Value.Value);

            var relations = await Task.WhenAll(relationIDs.Select(relation => GetPropertyProviderRelation(providerID, relation)));
            return relations;
        }

        public async Task<IEvent> GetPropertyProviderValueConstraint(int providerID, int propertyID, int constraintID) {
            await Task.Yield();

            var property = (
                from propValue in events
                join assignment in events on propValue.Value.BaseEventID equals assignment.Value.ID
                join propDef in events on propValue.Value.ValueID equals propDef.Key
                where
                    propDef.Value.BaseEventID == StaticEvent.ValueProperty &&
                    assignment.Value.BaseEventID == providerID &&
                    assignment.Value.Value == propertyID.ToString() &&
                    propValue.Value.ValueID == constraintID
                select propValue.Value
            ).SingleOrDefault();

            if (null == property) {
                return null;
            }

            return new Event(context, property);
        }

        public async Task<IEnumerable<IEvent>> GetPropertyProviderValueConstraints(int providerID, int propertyID) {
            await Task.Yield();

            var properties =
                from propValue in events
                join assignment in events on propValue.Value.BaseEventID equals assignment.Value.ID
                join propDef in events on propValue.Value.ValueID equals propDef.Key
                where
                    propDef.Value.BaseEventID == StaticEvent.ValueProperty &&
                    assignment.Value.BaseEventID == providerID &&
                    assignment.Value.Value == propertyID.ToString()
                select new Event(context, propValue.Value);

            return properties;
        }
        #endregion

        #region Property container
        public async Task<IPropertyContainer> GetPropertyContainer(int containerID) {
            var containerEvent = await GetEvent(containerID);
            var isAttr = await IsEventAncestor(StaticEvent.Attribute, containerEvent.EventValue.ValueID);
            var isRelation = await IsEventAncestor(StaticEvent.Relation, containerEvent.EventValue.ValueID);

            if (containerEvent.EventValue.ValueID != StaticEvent.Individual && !isAttr && !isRelation) {
                return null;
            }

            if (!containerProviders.TryGetValue(containerID, out var providerID)) {
                return null;
            }

            return new PropertyContainer(context, providerID, containerID);
        }

        public async Task<IPropertyProvider> GetContainerPropertyProvider(int containerID) {
            if (!containerProviders.TryGetValue(containerID, out var providerID)) {
                return null;
            }

            var providerEvent = await GetEvent(providerID);
            if (providerEvent.EventValue.ValueID == StaticEvent.Model) {
                var model = await GetModel(providerID);
                return new ModelPropertyProvider(context, model);
            }
            else {
                return new PropertyProvider(context, providerID);
            }
        }

        public async Task<IEnumerable<IBoxedValue>> GetPropertyContainerAttribute(int containerID, int attributeID) {
            var values =
                from e in events
                join subEvent in events
                on e.Value.ValueID
                equals subEvent.Key
                where
                    e.Value.BaseEventID == containerID &&
                    subEvent.Value.BaseEventID == StaticEvent.Attribute &&
                    subEvent.Value.ValueID == StaticEvent.Individual &&
                    subEvent.Value.ID == attributeID
                select e.Value;

            var attr = await GetAttribute(attributeID);
            var boxed = await attr.IsBoxed();
            if (boxed) {
                return await Task.WhenAll(values.Select(async e => {
                    var valueID = Int32.Parse(e.Value);
                    var valueEvent = await GetEvent(valueID);
                    return new BoxedValue(context, e, valueEvent.EventValue.Value);
                }));
            }
            else {
                return values.Select(e => new BoxedValue(context, e, e.Value));
            }
        }

        public async Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetPropertyContainerAttributes(int containerID) {
            var attributes =
                from e in events
                join subEvent in events
                on e.Value.ValueID
                equals subEvent.Key
                where
                    e.Value.BaseEventID == containerID &&
                    subEvent.Value.BaseEventID == StaticEvent.Attribute &&
                    subEvent.Value.ValueID == StaticEvent.Individual
                select new {
                    ID = subEvent.Value.ID,
                    Event = e.Value
                };

            var result = new Dictionary<int, IEnumerable<IBoxedValue>>();
            foreach (var data in attributes) {
                if (!result.ContainsKey(data.ID)) {
                    result.Add(data.ID, new List<IBoxedValue>());
                }

                var attr = await GetAttribute(data.ID);
                var boxed = await attr.IsBoxed();
                var list = (List<IBoxedValue>)result[data.ID];

                if (boxed) {
                    var valueID = Int32.Parse(data.Event.Value);
                    var valueEvent = await GetEvent(valueID);
                    list.Add(new BoxedValue(context, data.Event, valueEvent.EventValue.Value));
                }
                else {
                    list.Add(new BoxedValue(context, data.Event, data.Event.Value));
                }
            }

            return result;
        }

        public async Task<IEnumerable<IBoxedValue>> GetPropertyContainerRelation(int containerID, int relationID) {
            var values =
                from e in events
                join subEvent in events
                on e.Value.ValueID
                equals subEvent.Key
                where
                    e.Value.BaseEventID == containerID &&
                    subEvent.Value.BaseEventID == StaticEvent.Relation &&
                    subEvent.Value.ValueID == StaticEvent.Individual &&
                    subEvent.Value.ID == relationID
                select e.Value;

            return await Task.WhenAll(values.Select(async e => {
                var valueID = Int32.Parse(e.Value);
                var valueEvent = await GetEvent(valueID);
                return new BoxedValue(context, e, valueEvent.EventValue.Value);
            }));
        }

        public async Task<IReadOnlyDictionary<int, IEnumerable<IBoxedValue>>> GetPropertyContainerRelations(int containerID) {
            var relations =
                from e in events
                join subEvent in events
                on e.Value.ValueID
                equals subEvent.Key
                where
                    e.Value.BaseEventID == containerID &&
                    subEvent.Value.BaseEventID == StaticEvent.Relation &&
                    subEvent.Value.ValueID == StaticEvent.Individual
                select new {
                    ID = subEvent.Value.ID,
                    Event = e.Value
                };

            var result = new Dictionary<int, IEnumerable<IBoxedValue>>();
            foreach (var data in relations) {
                if (!result.ContainsKey(data.ID)) {
                    result.Add(data.ID, new List<IBoxedValue>());
                }

                var list = (List<IBoxedValue>)result[data.ID];
                var valueID = Int32.Parse(data.Event.Value);
                var valueEvent = await GetEvent(valueID);
                list.Add(new BoxedValue(context, data.Event, valueEvent.EventValue.Value));
            }

            return result;
        }
        #endregion

        #region Entity
        public async Task<IEntity> GetEntity(int entityID) {
            var entityDef = await GetEvent(entityID);

            if (entityDef.EventValue.ValueID != StaticEvent.SubEvent || entityDef.EventValue.BaseEventID != StaticEvent.Entity) {
                return null;
            }

            return new Entity(context, entityDef.EventValue);
        }

        public async Task<IEnumerable<IEntity>> GetEntities() {
            var entityIDs =
                from e in events
                where e.Value.ValueID == StaticEvent.SubEvent &&
                e.Value.BaseEventID == StaticEvent.Entity
                select e.Key;

            return await Task.WhenAll(entityIDs.Select(id => GetEntity(id)));
        }


        public async Task<IEnumerable<IModel>> GetEntityModels(int entityID) {
            var modelIDs =
                from e in events
                where e.Value.BaseEventID == entityID && e.Value.ValueID == StaticEvent.Model
                select e.Key;

            return await Task.WhenAll(modelIDs.Select(model => GetModel(model)));
        }

        public async Task<IEnumerable<IIndividual>> GetEntityIndividuals(int entityID) {
            var individualIDs =
                from e in events
                where e.Value.BaseEventID == entityID && e.Value.ValueID == StaticEvent.Individual
                select e.Key;

            return await Task.WhenAll(individualIDs.Select(individual => GetIndividual(individual)));
        }
        #endregion

        #region Individual
        public async Task<IIndividual> GetIndividual(int individualID) {
            var individualDef = await GetEvent(individualID);

            if (individualDef.EventValue.ValueID != StaticEvent.Individual) {
                return null;
            }

            return new Individual(context, individualDef.EventValue);
        }

        public async Task<IEnumerable<IIndividual>> GetIndividuals() {
            var individualIDs =
                from e in events
                where e.Value.ValueID == StaticEvent.Individual
                select e.Key;

            return await Task.WhenAll(individualIDs.Select(id => GetIndividual(id)));
        }
        #endregion

        #region Actor
        public async Task<IIndividual> GetActor(int actorID) {
            var actorDef = await GetEvent(actorID);

            if (null == actorDef || actorDef.EventValue.BaseEventID != StaticEvent.Actor || actorDef.EventValue.ValueID != StaticEvent.Individual) {
                return null;
            }

            return await GetIndividual(actorID);
        }

        public async Task<IEnumerable<IIndividual>> GetActors() {
            var individualIDs =
                from e in events
                where
                    e.Value.ValueID == StaticEvent.Individual &&
                    e.Value.BaseEventID == StaticEvent.Actor
                select e.Key;

            return await Task.WhenAll(individualIDs.Select(id => GetIndividual(id)));
        }
        #endregion

        #region Role
        public async Task<IIndividual> GetRole(int roleID) {
            var actorDef = await GetEvent(roleID);

            if (null == actorDef || actorDef.EventValue.BaseEventID != StaticEvent.Role || actorDef.EventValue.ValueID != StaticEvent.Individual) {
                return null;
            }

            return await GetIndividual(roleID);
        }
        public async Task<IEnumerable<IIndividual>> GetRoles() {
            var individualIDs =
                from e in events
                where
                    e.Value.ValueID == StaticEvent.Individual &&
                    e.Value.BaseEventID == StaticEvent.Role
                select e.Key;

            return await Task.WhenAll(individualIDs.Select(id => GetIndividual(id)));
        }
        #endregion

        #region Other
        public async Task<IIndividual> GetDataTypeIndividual(string name) {
            var ev = (
                from e in events
                where e.Value.Value == name &&
                e.Value.BaseEventID == StaticEvent.DataType &&
                e.Value.ValueID == StaticEvent.Individual
                select e
            ).SingleOrDefault();

            if (null == ev.Value) {
                return null;
            }
            else {
                return await GetIndividual(ev.Key);
            }
        }

        public DataType GetDataType(string name) {
            return context.Types.Get(name);
        }
        #endregion
    }
}