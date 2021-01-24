// Aurora 
// Copyright (C) 2020  Frank Horrigan <https://github.com/saurer>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Threading.Tasks;
using AuroraCore.Effects;
using AuroraCore.Storage;
using static AuroraCore.Storage.ConditionRule;

namespace AuroraCore.Controllers {
    public class EventController : Controller {
        [EventReaction(StaticEvent.Event)]
        public async Task<Effect> Event(IEventData e) {
            var existingEvent = await Storage.GetEvent(e.ID);
            if (null != existingEvent) {
                throw new Exception($"Event '{e.ID}' already exists");
            }

            var genesisEvent = await Storage.GetEvent(StaticEvent.Event);
            var baseEvent = await Storage.GetEvent(e.BaseEventID);
            if (null != genesisEvent && null == baseEvent) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            var valueEvent = await Storage.GetEvent(e.ValueID);
            if (null == valueEvent && null != genesisEvent) {
                throw new Exception($"Event '{e.ValueID}' does not exist");
            }

            foreach (var condition in TraverseConditions(e.Conditions)) {
                if (condition is ConditionRule.EventConditionRule eventCondition) {
                    var conditionEvent = await Storage.GetEvent(eventCondition.EventID);
                    if (0 != e.ID && null == conditionEvent) {
                        throw new Exception($"Event '{eventCondition.EventID}' does not exist");
                    }
                }
                else if (condition is ConditionRule.PropertyEqualityRule || condition is ConditionRule.PropertyInequalityRule) {
                    continue;
                }
                else {
                    throw new Exception($"Unhandled condition rule type: '{condition}'");
                }
            }

            return Effect.Pass;
        }

        [EventReaction(StaticEvent.SubEvent)]
        public async Task<Effect> SubEvent(IEventData e) {
            IEvent parent = await Storage.GetEvent(e.BaseEventID);

            if (null == parent) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            return new TagSubEventEffect(e.BaseEventID, e.ID);
        }

        [EventReaction(StaticEvent.AttributeConstraint)]
        public async Task<Effect> AttributeProperty(IEventData e) {
            IEvent parent = await Storage.GetEvent(e.BaseEventID);
            if (null == parent) {
                throw new Exception($"Base event {e.BaseEventID} does not exist");
            }

            int constraintDefID;
            if (!Int32.TryParse(e.Value, out constraintDefID)) {
                throw new Exception($"Expected ID: '{e.Value}'");
            }

            IEvent propertyDef = await Storage.GetEvent(constraintDefID);
            if (null == propertyDef) {
                throw new Exception($"Property definition {constraintDefID} does not exist");
            }

            if (parent.EventValue.ValueID == StaticEvent.Model) {
                if (parent.EventValue.BaseEventID != StaticEvent.Attribute) {
                    throw new Exception($"Only attributes can have properties");
                }
            }
            else if (parent.EventValue.BaseEventID == StaticEvent.Attribute) {
                var attr = await Storage.GetAttribute(e.BaseEventID);
                if (null == attr) {
                    throw new Exception("Attribute " + e.BaseEventID + " does not exist");
                }

                var constraint = await Storage.GetAttributeConstraint(e.ValueID);

                if (null == constraint) {
                    throw new Exception($"Attribute constaint '{e.ValueID}' does not exist");
                }

                var exists = await constraint.ContainsValueCandidate(constraintDefID);
                if (!exists) {
                    throw new Exception($"Value '{constraintDefID}' is not assignable to this event");
                }
            }
            else {
                throw new Exception("Illegal operation");
            }

            return Effect.Pass;
        }

        [EventReaction(StaticEvent.Model)]
        public async Task<Effect> Model(IEventData e) {
            EventConditionRule eventCondition = null;
            if (!TryGetCondition<ConditionRule.EventConditionRule>(e, out eventCondition)) {
                throw new Exception($"Model does not have a parent");
            }

            var parent = await Storage.GetEvent(eventCondition.EventID);
            if (null == parent) {
                throw new Exception($"Event '{eventCondition.EventID}' does not exist");
            }

            if (parent.EventValue.ValueID != StaticEvent.Model && parent.EventValue.ValueID != StaticEvent.Event) {
                throw new Exception("Model base can only be another model");
            }

            var parentModel = await Storage.GetModel(parent.EventValue.ID);
            if (eventCondition.EventID != StaticEvent.Event && null == parentModel) {
                throw new Exception("Model " + eventCondition.EventID + " does not exist");
            }

            return Effect.Pass;
        }

        [EventReaction(StaticEvent.Attribute)]
        public async Task<Effect> Attribute(IEventData e) {
            IEvent baseEvent = await Storage.GetEvent(e.BaseEventID);

            if (e.ValueID == StaticEvent.Attribute) {
                int attrID;

                if (!Int32.TryParse(e.Value, out attrID)) {
                    throw new Exception("Invalid attribute ID: " + e.Value);
                }

                var attr = await Storage.GetAttribute(attrID);
                if (null == attr) {
                    throw new Exception("Attribute " + attrID + " does not exist");
                }

                var existingAttr = await Storage.GetPropertyProviderAttribute(e.BaseEventID, attrID);
                if (existingAttr != null) {
                    throw new Exception($"Provider '{e.BaseEventID}' already has attribute '{attrID}'");
                }

                return Effect.Pass;
            }

            var container = await Storage.GetPropertyContainer(e.BaseEventID);
            var provider = await Storage.GetContainerPropertyProvider(e.BaseEventID);
            var providerBase = await Storage.GetPropertyProvider(e.BaseEventID);

            if (null != provider && null != container) {
                var attribute = await Storage.GetAttribute(e.ValueID);

                if (null == attribute) {
                    throw new Exception($"Event '{e.ValueID}' does not exist");
                }

                var boxed = await attribute.IsBoxed();
                if (boxed) {
                    if (Int32.TryParse(e.Value, out var valueID)) {
                        var valueIndividual = await Storage.GetAttributeValueCandidate(attribute.PropertyID, valueID);

                        if (null == valueIndividual) {
                            throw new Exception($"Attribute value '{valueID}' is not defined for attribute '{attribute.PropertyID}'");
                        }
                    }
                    else {
                        throw new Exception($"Invalid attribute value ID: '{e.Value}', expected number");
                    }
                }
                else {
                    var valid = await attribute.Validate(e.Value);
                    if (!valid) {
                        throw new Exception($"Invalid value for attribute '{attribute.Label}'");
                    }
                }

                var property = await provider.GetAttribute(e.ValueID);
                var cardinality = await property.GetCardinality();
                var mutable = await property.GetMutability();

                if (!mutable && cardinality != 0) {
                    var values = await container.GetAttribute(attribute.PropertyID);
                    if (cardinality <= values.Count()) {
                        throw new Exception($"Cardinality violation, attribute '{attribute.PropertyID}' already has maximum number of values");
                    }
                }

                var permission = await property.GetPermission();
                if (permission.HasValue && permission.Value != e.ActorEventID) {
                    throw new Exception($"Permission violation, write access for attribute '{attribute.PropertyID}' is denied for this actor");
                }

                return new TagContainerEffect(e.ID, property.AttachmentID);
            }
            else if (null != providerBase) {
                return new TagProviderEventEffect(e.BaseEventID, e.ValueID);
            }
            else {
                throw new Exception($"Event '{e.BaseEventID}' has no attached property provider");
            }
        }

        [EventReaction(StaticEvent.Individual)]
        public async Task<Effect> Individual(IEventData e) {
            var parentEvent = await Storage.GetEvent(e.BaseEventID);
            if (null == parentEvent) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }


            EventConditionRule conditionEvent = null;
            if (!TryGetCondition<ConditionRule.EventConditionRule>(e, out conditionEvent)) {
                throw new Exception("Individual does not have a model");
            }

            var model = await Storage.GetModel(conditionEvent.EventID);
            if (null == model) {
                throw new Exception("Model " + conditionEvent.EventID + " does not exist");
            }

            if (e.BaseEventID == StaticEvent.DataType) {
                var type = Storage.GetDataType(e.Value);
                if (null == type) {
                    throw new Exception($"Type '{e.Value}' does not exist");
                }
            }

            switch (e.BaseEventID) {
                case StaticEvent.Attribute:
                case StaticEvent.Actor:
                case StaticEvent.Role:
                case StaticEvent.DataType:
                case StaticEvent.Relation:
                    break;
                default:
                    switch (parentEvent.EventValue.BaseEventID) {
                        case StaticEvent.Entity:
                            break;
                        default:
                            throw new Exception($"Invalid individual base event");
                    }
                    break;
            }

            return new MultipleEffect(
                new TagSubEventEffect(e.BaseEventID, e.ID),
                new TagContainerEffect(e.ID, model.ModelID)
            );
        }

        [EventReaction(StaticEvent.DataType)]
        public async Task<Effect> DataType(IEventData e) {
            int typeID;
            if (!Int32.TryParse(e.Value, out typeID)) {
                throw new Exception("Invalid DataType: " + e.Value);
            }

            var typeValue = await Storage.GetIndividual(typeID);
            if (null == typeValue) {
                throw new Exception("DataType '" + e.Value + "' is not registered");
            }

            var dataType = Storage.GetDataType(typeValue.Label);
            if (null == dataType) {
                throw new Exception("Type '" + typeValue.Label + "' is not active");
            }

            return Effect.Pass;
        }

        [EventReaction(StaticEvent.AttributeValue)]
        public async Task<Effect> AttributeValue(IEventData e) {
            var attr = await Storage.GetAttribute(e.BaseEventID);

            if (null == attr) {
                throw new Exception($"Attribute '{e.BaseEventID}' does not exist");
            }

            var dt = await attr.GetDataType();
            if (!dt.AllowsBoxedValue(e.Value)) {
                throw new Exception($"Invalid value for attribute '{attr.Label}'");
            }

            return Effect.Pass;
        }

        [EventReaction(StaticEvent.ValueProperty)]
        public async Task<Effect> ValueProperty(IEventData e) {
            var baseEvent = await Storage.GetEvent(e.BaseEventID);

            if (StaticEvent.Attribute == baseEvent.EventValue.ValueID) {
                var attr = await Storage.GetAttribute(Int32.Parse(baseEvent.EventValue.Value));
                if (null == attr) {
                    throw new Exception($"Attribute '{baseEvent.EventValue.Value}' does not exist");
                }
            }
            else if (StaticEvent.Relation == baseEvent.EventValue.ValueID) {
                var relation = await Storage.GetRelation(Int32.Parse(baseEvent.EventValue.Value));
                if (null == relation) {
                    throw new Exception($"Relation '{baseEvent.EventValue.Value}' does not exist");
                }
            }
            else {
                throw new Exception($"Invalid base event value: '{e.BaseEventID}'");
            }

            switch (e.ValueID) {
                case StaticEvent.Required:
                    if (e.Value != "1" && e.Value != "0") {
                        throw new Exception($"Invalid value property value of type '{e.ValueID}': '{e.Value}'");
                    }
                    break;

                case StaticEvent.Cardinality:
                    if (!Int32.TryParse(e.Value, out var intValue) || intValue < 0) {
                        throw new Exception($"Invalid value property value of type '{e.ValueID}': '{e.Value}'");
                    }
                    break;

                case StaticEvent.Permission:
                    if (!Int32.TryParse(e.Value, out intValue) || intValue < 0) {
                        throw new Exception($"Invalid value property value of type '{e.ValueID}': '{e.Value}'");
                    }

                    var permissionActor = await Storage.GetActor(intValue);
                    if (null == permissionActor) {
                        throw new Exception($"Invalid value property value of type '{e.ValueID}': '{e.Value}'");
                    }
                    break;

                case StaticEvent.Set:
                    if (StaticEvent.Attribute == baseEvent.EventValue.ValueID) {
                        var attr = await Storage.GetAttribute(Int32.Parse(baseEvent.EventValue.Value));
                        var boxed = await attr.IsBoxed();

                        if (boxed) {
                            var candidates = await attr.GetValueCandidates();
                            if (!Int32.TryParse(e.Value, out intValue)) {
                                throw new Exception($"Invalid value property value of type '{e.ValueID}: '{e.Value}'");
                            }

                            if (!candidates.Any(l => l.EventValue.ID == intValue)) {
                                throw new Exception($"Invalid value property value of type '{e.ValueID}: '{e.Value}'");
                            }
                        }
                    }
                    break;

                case StaticEvent.Mutable:
                    var id = Int32.Parse(baseEvent.EventValue.Value);
                    var cardinalityEvent = await Storage.GetPropertyProviderValueConstraint(e.BaseEventID, id, StaticEvent.Cardinality);
                    if (
                        (null == cardinalityEvent && Const.DefaultCardinality != 1) ||
                        (null != cardinalityEvent && cardinalityEvent.EventValue.Value != "1")
                    ) {
                        throw new Exception($"Property can not be set mutable when cardinality is not set to 1");
                    }
                    break;

                default:
                    throw new Exception($"Unhandled value property type: '{e.ValueID}'");
            }

            return Effect.Pass;
        }

        [EventReaction(StaticEvent.Relation)]
        public async Task<Effect> Relation(IEventData e) {
            IEvent baseEvent = await Storage.GetEvent(e.BaseEventID);

            if (e.ValueID == StaticEvent.Relation) {
                int relationID;

                if (!Int32.TryParse(e.Value, out relationID)) {
                    throw new Exception("Invalid relation ID: " + e.Value);
                }

                var relation = await Storage.GetRelation(relationID);
                if (null == relation) {
                    throw new Exception("Relation " + relationID + " does not exist");
                }

                var existingRelation = await Storage.GetPropertyProviderRelation(e.BaseEventID, relationID);
                if (existingRelation != null) {
                    throw new Exception($"Provider '{e.BaseEventID}' already has relation '{relationID}'");
                }

                return Effect.Pass;
            }

            var container = await Storage.GetPropertyContainer(e.BaseEventID);
            var provider = await Storage.GetContainerPropertyProvider(e.BaseEventID);
            var providerBase = await Storage.GetPropertyProvider(e.BaseEventID);

            if (null != provider && null != container) {
                var relation = await Storage.GetRelation(e.ValueID);

                if (null == relation) {
                    throw new Exception($"Event '{e.ValueID}' does not exist");
                }

                if (Int32.TryParse(e.Value, out var valueID)) {
                    var valueIndividual = await Storage.GetIndividual(valueID);

                    if (null == valueIndividual) {
                        throw new Exception($"Individual '{valueID}' does not exist");
                    }
                }
                else {
                    throw new Exception($"Invalid relation value ID: '{e.Value}', expected number");
                }

                var property = await provider.GetRelation(e.ValueID);
                var cardinality = await property.GetCardinality();
                var mutable = await property.GetMutability();

                if (!mutable && cardinality != 0) {
                    var values = await container.GetRelation(relation.PropertyID);
                    if (cardinality <= values.Count()) {
                        throw new Exception($"Cardinality violation, relation '{relation.PropertyID}' already has maximum number of values");
                    }
                }

                var permission = await property.GetPermission();
                if (permission.HasValue && permission.Value != e.ActorEventID) {
                    throw new Exception($"Permission violation, write access for relation '{property.PropertyID}' is denied for this actor");
                }

                return new TagContainerEffect(e.ID, property.AttachmentID);
            }
            else if (null != providerBase) {
                return new TagProviderEventEffect(e.BaseEventID, e.ValueID);
            }
            else {
                throw new Exception($"Event '{e.BaseEventID}' has no attached property provider");
            }
        }
    }
}